﻿using Framework.Features.UDP;
using Framework.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class Videocaller : MonoBehaviour, INetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;

	private UDPMaster udpMaster;


	// Sending Locals
	public WebCamTexture OwnFootage { get; private set; }
	private Stopwatch stopwatch = new Stopwatch();

	private string targetIP;
	private int frameRate;
	private float resolutionScale;


	// Receiving Locals
	public Texture2D OtherFootage;

	private int colorStartIndexX = 0;
	private int colorStartIndexY = 0;
	private bool dimensionsEstablished = false;


	public void Initialize(int portA = 11002, int portB = 11003)
	{
		resolutionScale = Mathf.Clamp01(resolutionScale);

		udpMaster = new UDPMaster();
		udpMaster.Initialize(PortA, PortB);
		udpMaster.AddListener(this);
		udpMaster.LocalHost = true; //TODO don't forget to remove this.

		// HACK: This will most definitely break at some point. 
		WebCamDevice frontCam = WebCamTexture.devices.Where((WebCamDevice d) => d.isFrontFacing).ToArray()[0];
		OwnFootage = new WebCamTexture(frontCam.name);
		OwnFootage.Play();
	}


	public void StartCalling(string targetIP, int frameRate, float resolutionScale)
	{
		this.targetIP = targetIP;
		this.frameRate = frameRate;
		this.resolutionScale = resolutionScale;

		udpMaster.UpdateTargetIP(targetIP);

		StopAllCoroutines();
		StartCoroutine(SendFootage());
	}

	private IEnumerator<YieldInstruction> SendFootage()
	{
		int targetDeltaTimeInMilliseconds = (int)((1f / frameRate) * 1000);

		while (true)
		{
			stopwatch.Restart();

			Color32[] pixels = OwnFootage.GetPixels32();

			// TODO: Do this on a different thread (framerate and such)?

			// Converts the width and height to byte array and sends it across the network.
			List<byte> resolutionByteList = new List<byte>();
			// TODO: Double check whether this should be ceil or something different.
			resolutionByteList.AddRange(Mathf.CeilToInt(OwnFootage.width * resolutionScale).ToByteArray());
			resolutionByteList.AddRange(Mathf.CeilToInt(OwnFootage.height * resolutionScale).ToByteArray());
			udpMaster.SendMessage(resolutionByteList.ToArray());
			
			// Reduces the resolution by the resolutionScale.
			List<Color32> filteredPixels = new List<Color32>();
			float stepSize = 1f / resolutionScale;
			for (float i = 0; i < OwnFootage.width; i += stepSize)
			{
				for (float j = 0; j < OwnFootage.height; j += stepSize)
				{
					int k = Mathf.RoundToInt(i * OwnFootage.height + j);
					Color32 pixel = pixels[k];
					filteredPixels.Add(pixel);
				}
			}

			int colorBufferSize = Mathf.FloorToInt(udpMaster.MessageBufferSize / 3f);
			// one color contains 3 bytes (rgb); 3 will recur during this method a few times. 
			int chunkCount = Mathf.CeilToInt((float)filteredPixels.Count / colorBufferSize);
			for (int i = 0; i < chunkCount; i++)
			{
				int j = i * colorBufferSize;
				int length = i == chunkCount - 1 // is the last chunk.
					? filteredPixels.Count - j
					: colorBufferSize; 


				// TODO: remove this sublist. Just iterate from the right indices.
				List<Color32> pixelSubList = filteredPixels.SubList(j, length);
				byte[] byteArray = new byte[pixelSubList.Count * 3];

				for (int k = 0; k < pixelSubList.Count; k++)
				{
					int l = k * 3;
					byteArray[l] = pixelSubList[k].r;
					byteArray[l + 1] = pixelSubList[k].g;
					byteArray[l + 2] = pixelSubList[k].b;
				}

				udpMaster.SendMessage(byteArray);
				yield return new WaitForEndOfFrame();
			}

			stopwatch.Stop();
			int timeLeft = targetDeltaTimeInMilliseconds - stopwatch.Elapsed.Milliseconds;
			timeLeft = Mathf.Min(0, timeLeft);
			yield return new WaitForEndOfFrame();
		}
	}


	public void StopCalling()
	{
		udpMaster.SendMessage(new byte[0]);
		StopAllCoroutines();
		udpMaster.Kill();
		OwnFootage.Stop();
		dimensionsEstablished = false;
	}


	public void OnMessageReceived(byte[] message)
	{
		// means the end of the conversation. Is sent in "StopCalling"
		if (message.Length == 0)
		{
			StopCalling(); // TODO: this is not proper solution as other classes cannot hook to it. 
			return;
		}

		// if they aren't they are set. 
		if (!dimensionsEstablished)
		{
			int INT_BYTEARRAYLENGTH = ObjectUtilities.INT_BYTEARRAYLENGTH; 
			int width = message.SubArray(0, INT_BYTEARRAYLENGTH).ToObject<int>();
			int height = message.SubArray(INT_BYTEARRAYLENGTH, INT_BYTEARRAYLENGTH).ToObject<int>();

			// Creates the texture.
			OtherFootage = new Texture2D(width, height);
			dimensionsEstablished = true;
			return;
		}

		// Applies the colors.
		for (int i = 0; i < message.Length; i+= 3)
		{
			// converts the byte info to a Color32 and sets it.
			byte r = message[i];
			byte g = message[i + 1];
			byte b = message[i + 2];
			Color32 color = new Color32(r, g, b, byte.MaxValue);

			OtherFootage.SetPixel(colorStartIndexX, colorStartIndexY, color);

			colorStartIndexX++;
			if (colorStartIndexX >= OtherFootage.width)
			{
				colorStartIndexX = 0;
				colorStartIndexY++;

				if (colorStartIndexY >= OtherFootage.height)
				{
					colorStartIndexY = 0;
					// Should this be called only at the end? UDP messages can get lost..
					OtherFootage.Apply();
					dimensionsEstablished = false;
				}
			}
		}
	}
}
