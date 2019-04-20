using Framework.Features.UDP;
using Framework.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Videocall : MonoBehaviour, INetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;

	[Space]
	public SharedFloat StreamingResolutionScale;
	public SharedInt StreamingFramerate;

	[Space]
	public RawImage imageOut;
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

		// TODO: load video streaming settings.
		textureOut = new Texture2D(600, 900);
		imageOut.texture = textureOut;

		if (swappedPorts)
			udpMaster.Initialize(PortA, PortB);
		else
			udpMaster.Initialize(PortB, PortA);

		udpMaster.UpdateTargetIP(other.IP);

		udpMaster.AddListener(this);

		StopCoroutine(CameraStream());
		StartCoroutine(CameraStream());
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
