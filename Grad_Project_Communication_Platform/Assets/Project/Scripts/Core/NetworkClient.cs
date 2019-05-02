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

		RoleplayController.ForceEndCall();

		base.Stop();
	}


	[ExecuteOnMainThread]
	public void TransmitRoleplayDescription(NetworkMessage message)
	{
		RoleplayController.OnRoleplayLoaded(message.Message);
	}

	public void TransmitFinalEvaluation(NetworkMessage message)
	{

	}
}
