using Framework.ScriptableObjects.Variables;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

public sealed class NetworkClient : NetworkManager
{
	[Space]
	[SerializeField] private SharedString AccountName;
	[SerializeField] private SharedString AccountPhone;
	[SerializeField] private SharedString ServerIP;

	[Space]
	public ScreenController ScreenController;
	public RoleplayCall RoleplayCall;
	public RoleplayController RoleplayController;
	public ModuleController ModuleController;

	public string ClientId { get { return AccountPhone.Value; } } 
	public string ClientName { get { return AccountName.Value; } }


	protected override void Awake()
	{
		base.Awake();
		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);
	}

	//private void Start()
	//{
	//	NetworkMessage asdf = new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", ClientId, JsonUtility.ToJson(new RoleplayDescription("", new Participant("Steve", "192.168.178.38", "123456789"), new Participant("Stevette", "192.168.178.18", "1346"), new CaseDescription(new int[] { 1 }, new int[] { 1 }, RoleplayModule.Paraphrasing))));
	//	TransmitRoleplayDescription(asdf);
	//}

	protected override void Initialize()
	{
		base.Initialize();
		udpMaster.LogReceivedMessages = true;
		udpMaster.UpdateTargetIP(ServerIP.Value);
		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountPhone.Value, "", AccountName.Value);
		SendMessage(connectMessage);
	}

	protected override void Stop()
	{
		NetworkMessage disconnectMessage = new NetworkMessage(NetworkMessageType.DisconnectFromServer, ClientId);
		SendMessage(disconnectMessage);

		RoleplayCall.ForceEndCall();

		base.Stop();
	}


	public void ConnectToServer(NetworkMessage message)
	{

	}
	
	[ExecuteOnMainThread]
	public void TransmitRoleplayDescription(NetworkMessage message)
	{
		RoleplayDescription roleplayDescription = JsonUtility.FromJson<RoleplayDescription>(message.Message);

		bool isClient = roleplayDescription.UserA.Id == ClientId;

		Participant other;
		Participant self; 

		if (isClient)
		{
			other = roleplayDescription.UserB;
			self = roleplayDescription.UserA;
		}
		else
		{
			other = roleplayDescription.UserA;
			self = roleplayDescription.UserB;
		}

		RoleplayCall.Initialize(isClient, other, self);
		RoleplayController.OnRoleplayLoaded(roleplayDescription, isClient);
	}

	public void TransmitFinalEvaluation(NetworkMessage message)
	{

	}

	public void TransmitFootage(NetworkMessage message)
	{

	}

	public void ForceDisconnect(NetworkMessage message)
	{

	}
}
