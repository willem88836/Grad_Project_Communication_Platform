using Framework.Features.UDP;
using Framework.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Videocall : MonoBehaviour, INetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;

	[Space]
	public SharedFloat StreamingResolutionScale;
	public SharedInt StreamingFramerate;

	[Space]
	public Material imageOut;
	private Texture2D textureOut;

	private UDPMaster<VideoMessage> udpMaster;
	private NetworkClient networkClient;

	private Participant other;


	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
		udpMaster = new UDPMaster<VideoMessage>();
	}


	public void StartCalling(bool swappedPorts, Participant other)
	{
		this.other = other;

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
		imageOut.mainTexture = textureOut;
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

		textureOut = null;

		networkClient.ForceEndCall(null);
	}


	public void OnMessageReceived(UDPMessage message)
	{
		VideoMessage videoMessage = (VideoMessage)message;
		Color32[] colors = videoMessage.Colors;
		textureOut.SetPixels32(colors);
		textureOut.Apply();
	}


	private IEnumerator<WaitForSeconds> CameraStream()
	{
		yield return new WaitForSeconds(1);
	}
}
