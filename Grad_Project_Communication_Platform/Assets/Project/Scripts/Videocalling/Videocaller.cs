using Framework.Features.UDP;
using Framework.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Videocaller : MonoBehaviour, INetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;


	private UDPMaster udpMaster;

	// Sending
	public WebCamTexture OwnFootage;

	private Thread senderThread;
	private Stopwatch stopwatch = new Stopwatch();

	private string targetIP;
	private int frameRate;
	private float resolutionScale;


	// Receiving
	//public Texture2D OtherFootage;


	public void Initialize(int portA = 11002, int portB = 11003)
	{
		udpMaster = new UDPMaster();
		udpMaster.Initialize(PortA, PortB);
		udpMaster.AddListener(this);

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
					: colorBufferSize; // ^ ditto


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
		StopAllCoroutines();
		senderThread.Abort();
		udpMaster.Kill();
	}


	public void OnMessageReceived(byte[] message)
	{

	}
}
