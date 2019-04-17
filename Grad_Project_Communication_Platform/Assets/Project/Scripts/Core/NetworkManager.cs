using Framework.Features.UDP;
using System.Reflection;
using UnityEngine;

public abstract class NetworkManager : MonoBehaviour, INetworkListener
{
	public int PortIn = 11000;
	public int PortOut = 11001;

	protected UDPMaster<NetworkMessage> udpMaster;

	private NetworkLogger<NetworkMessage> networkLogger; 


	protected virtual void Awake()
	{
		udpMaster = new UDPMaster<NetworkMessage>();
		udpMaster.Initialize(PortOut, PortIn);
		udpMaster.AddListener(this);


		networkLogger = new NetworkLogger<NetworkMessage>() { LogFileName = "NetworkLogs"};
		networkLogger.Initialize(udpMaster);
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
		// TODO: this is probably quite intensive. aka, fix this. 
		message.SenderIP = udpMaster.GetLocalIP().ToString(); 

		// TODO: do something with selecting Server IP or client IP here.
		udpMaster.SendMessage(message);
	}

	public void SendMessage(NetworkMessage message, string targetIP)
	{
		udpMaster.UpdateTargetIP(targetIP);
		SendMessage(message);
	}
}
