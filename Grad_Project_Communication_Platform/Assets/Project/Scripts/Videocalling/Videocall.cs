using Framework.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Framework.Features.UDP.Applied;

public class Videocall : MonoBehaviour, IAppliedNetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;

	[Space]
	public SharedFloat StreamingResolutionScale;
	public SharedInt StreamingFramerate;

	[Space]
	public Material OwnFootageOut;
	public Material OtherFootageOut;

	private AppliedUDPMaster<VideoMessage> udpMaster;
	private NetworkClient networkClient;

	private Participant other;
	private Participant self;


	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
		udpMaster = new AppliedUDPMaster<VideoMessage>();
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


	public void OnMessageReceived(UDPMessage message)
	{
		VideoMessage videoMessage = (VideoMessage)message;
		Color32[] colors = videoMessage.Colors;
		Texture2D videoOut = new Texture2D(videoMessage.Width, videoMessage.Height);
		videoOut.SetPixels32(colors);
		OtherFootageOut.mainTexture = videoOut;
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

			WebCamTexture texture2D = OwnFootageOut.mainTexture as WebCamTexture;

			VideoMessage videoMessage = new VideoMessage(
				self.IP, 
				texture2D.GetPixels32(), 
				texture2D.width, 
				texture2D.height);

			List<byte> byteList = new List<byte>();

			Color32[] colors = videoMessage.Colors;
			for (int i = 0; i < colors.Length; i++)
			{
				Color32 clr = colors[i];

				byteList.Add(clr.r);
				byteList.Add(clr.g);
				byteList.Add(clr.b);
			}

			byte[] videoByteArray = byteList.ToArray();


			// todo: send message here. 

			stopwatch.Stop();
			float timeLeft = targetDeltaTime - stopwatch.Elapsed.Seconds;
			Debug.LogFormat("Finished with {0} seconds left", timeLeft);
			timeLeft = Mathf.Clamp(timeLeft, 0, timeLeft);
			Debug.LogFormat("Completed early! Waiting for {0} seconds", timeLeft.ToString());

			yield return new WaitForSeconds(timeLeft);
		}
	}
}
