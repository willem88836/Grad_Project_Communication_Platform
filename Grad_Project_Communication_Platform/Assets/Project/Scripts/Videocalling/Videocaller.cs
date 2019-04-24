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

	private int targetDeltaTimeInMilliseconds;
	private int videoChunkCount;


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

		this.targetDeltaTimeInMilliseconds = (int)((1f / frameRate) * 1000);
		this.videoChunkCount = Mathf.CeilToInt(((OwnFootage.width / resolutionScale) + (OwnFootage.height / resolutionScale)) / udpMaster.MessageBufferSize);

		udpMaster.UpdateTargetIP(targetIP);

		StartSendingFootage();
	}

	public void StopCalling()
	{
		senderThread.Abort();
		udpMaster.Kill();
	}


	private void StartSendingFootage()
	{
		StartCoroutine(SendFootage());
		//senderThread = new Thread(activity);
		//senderThread.Start();
	}

	private IEnumerator<YieldInstruction> SendFootage()
	{
		while(true)
		{
			stopwatch.Restart();

			Color32[] pixels = OwnFootage.GetPixels32();

			for (int i = 0; i < videoChunkCount; i++)
			{
				int j = i * udpMaster.MessageBufferSize;
				int length = i == videoChunkCount - 1
					? pixels.Length - j
					: udpMaster.MessageBufferSize;


				Color32[] pixelSubArray = pixels.SubArray(j, length);
				byte[] byteArray = new byte[pixelSubArray.Length * 3];

				for (int k = 0; k < pixelSubArray.Length; k++)
				{
					int l = k * 3;
					byteArray[l] = pixelSubArray[k].r;
					byteArray[l + 1] = pixelSubArray[k].g;
					byteArray[l + 2] = pixelSubArray[k].b;
				}

				udpMaster.SendMessage(byteArray);
				yield return new WaitForEndOfFrame();
			}

			stopwatch.Stop();
			int timeLeft = targetDeltaTimeInMilliseconds - stopwatch.Elapsed.Milliseconds;
			timeLeft = Mathf.Min(0, timeLeft);
			//Thread.Sleep(timeLeft);
			yield return new WaitForEndOfFrame();
		}
	}


	public void OnMessageReceived(byte[] message)
	{

	}
}
