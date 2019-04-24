using Framework.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Framework.Features.UDP;
using Framework.Variables;
using Framework.Utils;

public class Videocall : MonoBehaviour, INetworkListener
{
	private const int SHORT_BYTELENGTH = 52;

	public int PortA = 11002;
	public int PortB = 11003;

	[Space]
	public SharedFloat StreamingResolutionScale;
	public SharedInt StreamingFramerate;

	[Space]
	public Material OwnFootageOut;
	public Material OtherFootageOut;

	private UDPMaster udpMaster;
	private NetworkClient networkClient;

	private Participant other;
	private Participant self;



	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
		udpMaster = new UDPMaster();
	}


	public void StartCalling(bool swappedPorts, Participant other, Participant self)
	{
		this.other = other;
		this.self = self;

		InitializeWebcam();

		InitializeUDP(swappedPorts, other.IP);

		StopCoroutine(CameraStream());
		StartCoroutine(CameraStream());
	}

	private void InitializeUDP(bool swappedPorts, string targetIP)
	{
		if (swappedPorts)
			udpMaster.Initialize(PortA, PortB);
		else
			udpMaster.Initialize(PortB, PortA);

		udpMaster.UpdateTargetIP(other.IP);

		udpMaster.AddListener(this);
	}
	
	private void InitializeWebcam()
	{
		// HACK: This will most definitely break at some point. 
		WebCamDevice frontCam = WebCamTexture.devices.Where((WebCamDevice d) => d.isFrontFacing).ToArray()[0];
		WebCamTexture textureOut = new WebCamTexture(frontCam.name);
		textureOut.Play();
		OwnFootageOut.mainTexture = textureOut;
	}


	public void StopCalling()
	{
		NetworkMessage forceEndCallMessage = new NetworkMessage(NetworkMessageType.ForceEndCall, networkClient.ClientId, other.Id);
		networkClient.SendMessage(forceEndCallMessage, other.IP);
		networkClient.ForceEndCall(forceEndCallMessage);
	}

	public void ForceEndCalling()
	{
		StopCoroutine(CameraStream());

		udpMaster.RemoveListener(this);
		udpMaster.Kill();

		OtherFootageOut = null;

		networkClient.ForceEndCall(null);
	}


	public void OnMessageReceived(byte[] message)
	{
		// Grabs the width and height from the array and converts them to height. 
		int width = message.SubArray(message.Length - (2 * SHORT_BYTELENGTH), SHORT_BYTELENGTH).ToObject<int>();
		int height = message.SubArray(message.Length - SHORT_BYTELENGTH, SHORT_BYTELENGTH).ToObject<int>();
		Texture2D videoOut = new Texture2D(width, height);
		

		List<Color32> colors = new List<Color32>();

		for (int i = 0; i < message.Length - (2 * SHORT_BYTELENGTH); i += 3)
		{
			Color32 pixel = new Color32(
				message[i], 
				message[i + 1], 
				message[i + 2], 
				byte.MaxValue);

			colors.Add(pixel);
		}

		Color32[] colorArray = colors.ToArray();
		videoOut.SetPixels32(colorArray);
		OtherFootageOut.mainTexture = videoOut;
		videoOut.Apply();
	}


	private IEnumerator<YieldInstruction> CameraStream()
	{
		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
		float targetDeltaTime = 1f / StreamingFramerate.Value;
		Debug.LogFormat("Target DeltaTime = {0}", targetDeltaTime);

		while (true)
		{
			stopwatch.Reset();
			stopwatch.Start();

			// TODO: See if this can be threaded. 
			// TODO: Do something to reduce the RAM use.
			WebCamTexture webCamTexture = OwnFootageOut.mainTexture as WebCamTexture;
			List<byte> byteList = new List<byte>();

			Color32[] colors = webCamTexture.GetPixels32();
			float stepSize = 1f / StreamingResolutionScale.Value;
			for (float i = 0; i < webCamTexture.width; i += stepSize)
			{
				for (float j = 0; j < webCamTexture.height; j += stepSize)
				{
					int k = Mathf.FloorToInt(j) * webCamTexture.width + (int)i;
					Color32 clr = colors[k];

					byteList.Add(clr.r);
					byteList.Add(clr.g);
					byteList.Add(clr.b);
				}
			}

			// Adds width and height of the webcamtexture.
			byteList.AddRange(((short)(webCamTexture.width * StreamingResolutionScale.Value)).ToByteArray());
			byteList.AddRange(((short)(webCamTexture.height * StreamingResolutionScale.Value)).ToByteArray());

			byte[] videoByteArray = byteList.ToArray();

			int bufferSize = udpMaster.MessageBufferSize;
			int chunkCount = Mathf.CeilToInt((float)videoByteArray.Length / bufferSize);
			for (int i = 0; i < chunkCount; i++)
			{
				int startIndex = i * bufferSize;
				int length = startIndex + bufferSize >= videoByteArray.Length 
					? videoByteArray.Length - startIndex 
					: bufferSize;
				byte[] byteSubArray = videoByteArray.SubArray(startIndex, length);
				udpMaster.SendMessage(byteSubArray, other.IP);
			}

			stopwatch.Stop();
			float timeLeft = targetDeltaTime - stopwatch.Elapsed.Seconds;
			timeLeft = Mathf.Clamp(timeLeft, 0, timeLeft);

			yield return new WaitForSeconds(timeLeft);
		}
	}
}
