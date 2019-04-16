using Framework.Features.UDP;
using System.Reflection;
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
		udpMaster.Initialize(PortOut, PortIn);
		udpMaster.AddListener(this);
	}

	protected virtual void OnDestroy()
	{
		udpMaster.RemoveListener(this);
		udpMaster.Kill();
	}


	public void OnMessageReceived(UDPMessage message)
	{
		// Calls the function corresponding with the message's type.
		NetworkMessage netMsg = (NetworkMessage)message;
		string methodName = netMsg.Type.ToString();
		MethodInfo method = GetType().GetMethod(methodName);
		method.Invoke(this, new object[] { netMsg });
	}

	public void SendMessage(NetworkMessage message)
	{
		// TODO: do something with selecting Server IP or client IP here.
		udpMaster.SendMessage(message);
	}
}
