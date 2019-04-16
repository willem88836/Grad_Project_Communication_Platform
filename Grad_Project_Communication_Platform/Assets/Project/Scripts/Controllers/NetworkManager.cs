using Framework.Features.UDP;
using System;
using UnityEngine;

public abstract class NetworkManager : MonoBehaviour, INetworkListener
{
	public int PortIn = 11000;
	public int PortOut = 11001;

	private UDPMaster<NetworkMessage> udpMaster;

	protected string Id;

	protected virtual void Awake()
	{
		udpMaster = new UDPMaster<NetworkMessage>();
		udpMaster.Initialize(PortIn, PortOut);
		udpMaster.AddListener(this);
	}

	protected virtual void OnDestroy()
	{
		udpMaster.RemoveListener(this);
		udpMaster.Kill();
	}


	public void OnMessageReceived(UDPMessage message)
	{
		throw new NotImplementedException();
	}

	public void SendMessage(NetworkMessage message)
	{
		// TODO: do something with selecting Server IP or client IP here.
		udpMaster.SendMessage(message);
	}
}
