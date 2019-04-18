using Framework.ScriptableObjects.Variables;
using UnityEngine;

public sealed class NetworkClient : NetworkManager
{
	[SerializeField] private SharedString AccountName;
	[SerializeField] private SharedString AccountPhone;

	public RoleplayController RoleplayController;
	public ModuleController ModuleController;


	public string ClientId { get { return AccountPhone.Value; } } 
	public string ClientName { get { return AccountName.Value; } }


	protected override void Awake()
	{
		base.Awake();

		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);

		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountName.Value, "", AccountName.Value);
		SendMessage(connectMessage);
	}

	protected override void OnDestroy()
	{
		NetworkMessage disconnectMessage = new NetworkMessage(NetworkMessageType.DisconnectFromServer, ClientId);
		SendMessage(disconnectMessage);
		base.OnDestroy();
	}


	public void ConnectToServer(NetworkMessage message)
	{
		// TODO: store this somewhere properly when we have to switch between sending messages to server to client.
		string serverIP = message.SenderIP;
		udpMaster.UpdateTargetIP(serverIP);
	}

	public void TransmitRoleplayDescription(NetworkMessage message)
	{

	}

	public void TransmitFinalEvaluation(NetworkMessage message)
	{

	}

	public void TransmitFootage(NetworkMessage message)
	{

	}

	public void ForceEndCall(NetworkMessage message)
	{

	}

	public void ForceDisconnect(NetworkMessage message)
	{

	}
}
