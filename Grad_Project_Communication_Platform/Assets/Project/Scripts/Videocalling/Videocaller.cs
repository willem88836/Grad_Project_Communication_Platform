using Framework.Features.UDP;
using Framework.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

/// <summary>
///		Is used for videocalling with someone across the network in Unity.
/// </summary>
public class Videocaller : MonoBehaviour, INetworkListener
{
	public Action OnCallEnded;
	//HACK: Changing the width/height of a Texture2D is not supported yet.
	public Action<Texture2D> OnOtherFootageApplied; 

	// Networking locals.
	public int PortA = 11002;
	public int PortB = 11003;

	private UDPMaster udpMaster;


	// Sending Locals
	public WebCamTexture OwnFootage { get; private set; }

	private string targetIP;
	private int frameRate;
	private float resolutionScale;


	// Receiving Locals
	public Texture2D OtherFootage { get; private set; }

	private int processedColors = 0;
	private bool dimensionsEstablished = false;


	/// <summary>
	///		Initializes the network connection.
	/// </summary>
	public void Initialize(int portA = 11002, int portB = 11003)
	{
		resolutionScale = Mathf.Clamp01(resolutionScale);

		udpMaster = new UDPMaster();
		udpMaster.Initialize(PortA, PortB);
		udpMaster.AddListener(this);

		// HACK: This will most definitely break at some point. 
		WebCamDevice frontCam = WebCamTexture.devices.Where((WebCamDevice d) => d.isFrontFacing).ToArray()[0];
		OwnFootage = new WebCamTexture(frontCam.name);
		OwnFootage.name = "Webcamfootage_Self";
		OwnFootage.Play();
	}

	/// <summary>
	///		Starts a videocall.
	/// </summary>
	public void StartCalling(string targetIP, int frameRate, float resolutionScale)
	{
		this.targetIP = targetIP;
		this.frameRate = frameRate;
		this.resolutionScale = resolutionScale;

		udpMaster.UpdateTargetIP(targetIP);

		StopAllCoroutines();
		StartCoroutine(SendFootage());
	}

	/// <summary>
	///		Collects the own video footage, reduces resolution, 
	///		and sends it across the network. 
	/// </summary>
	private IEnumerator<YieldInstruction> SendFootage()
	{
		// TODO: Do this on a different thread (framerate and such)?
		while (true)
		{
			int ownFootageWidth = OwnFootage.width;
			int ownFootageHeight = OwnFootage.height;

			// Converts the width and height to byte array and sends it across the network.
			List<byte> resolutionByteList = new List<byte>();
			int videoWidth = (int)(ownFootageWidth * resolutionScale);
			int videoHeight = (int)(ownFootageHeight * resolutionScale);
			resolutionByteList.AddRange(videoWidth.ToByteArray());
			resolutionByteList.AddRange(videoHeight.ToByteArray());
			udpMaster.SendMessage(resolutionByteList.ToArray());

			// Lowers the resolution of the video frame.
			Color32[] frame = OwnFootage.GetPixels32();
			List<Color32> lowResFrame = new List<Color32>();
			for (float i = 0; i < videoHeight; i++)
			{
				for (float j = 0; j < videoWidth; j++)
				{
					int x = (int)(j / resolutionScale);
					int y = (int)(i / resolutionScale);

					int k = y * ownFootageWidth + x;

					Color32 pixel = frame[k];
					lowResFrame.Add(pixel);
				}
			}
			
			// Sends the low resolution frame in chunks across the network.
			// Color32 contains 3 byte values (RGB). Therefore, everything is done in 3s.
			int colorBufferSize = (int)(udpMaster.MessageBufferSize / 3f);
			int chunkCount = Mathf.CeilToInt((float)lowResFrame.Count / colorBufferSize);
			for (int i = 0; i < chunkCount; i++)
			{
				// Establishes the message size. 
				int j = i * colorBufferSize;
				int length = i == chunkCount - 1 // Is the last chunk.
					? lowResFrame.Count - j
					: colorBufferSize; 

				// Creates the message.
				byte[] byteArray = new byte[lowResFrame.Count * 3];
				for (int k = 0; k < length; k++)
				{
					int l = j + k * 3;
					byteArray[l] = lowResFrame[k].r;
					byteArray[l + 1] = lowResFrame[k].g;
					byteArray[l + 2] = lowResFrame[k].b;
				}

				udpMaster.SendMessage(byteArray);
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	///		Ends the videocall. 
	/// </summary>
	public void StopCalling()
	{
		udpMaster.SendMessage(new byte[0]);
		StopAllCoroutines();
		udpMaster.Kill();
		OwnFootage.Stop();
		dimensionsEstablished = false;
		OnCallEnded.SafeInvoke();
	}

	/// <inheritdoc />
	public void OnMessageReceived(byte[] message)
	{
		// An empty array means that the call is ended.
		// This message is only sent in the StopCalling method. 
		if (message.Length == 0)
		{
			StopCalling();
			return;
		}

		// if the dimensions aren't established yet, 
		// it means the message contains that information.
		if (!dimensionsEstablished)
		{
			// Converts the byte array to two values.
			int intByteArrayLength = ObjectUtilities.INT_BYTEARRAYLENGTH; 
			int width = message.SubArray(0, intByteArrayLength).ToObject<int>();
			int height = message.SubArray(intByteArrayLength, intByteArrayLength).ToObject<int>();

			// Creates the texture.
			OtherFootage = new Texture2D(width, height);
			OtherFootage.name = "Webcamfootage_Other";
			dimensionsEstablished = true;
			processedColors = 0;
			return;
		}

		// Applies the colors to the texture.
		int otherFootageWidth = OtherFootage.width;
		int otherFootageHeight = OtherFootage.height;
		for (int i = 0; i < message.Length; i+= 3)
		{
			// Converts the byte info to Color32.
			byte r = message[i];
			byte g = message[i + 1];
			byte b = message[i + 2];
			Color32 color = new Color32(r, g, b, byte.MaxValue);

			// Determines the x and y coordinates of the color.
			int j = (int)(((processedColors + i) / 3f) % otherFootageWidth);
			int k = (int)(((processedColors + i) / 3f) / otherFootageWidth);
			OtherFootage.SetPixel(j, k, color);

			// Applies the new texture if it is the final chunk.
			if (k == otherFootageHeight - 1 && j == otherFootageWidth - 1)
			{
				OtherFootage.Apply();
				dimensionsEstablished = false;
				OnOtherFootageApplied.SafeInvoke(OtherFootage);
			}
		}

		processedColors += message.Length;
	}
}
