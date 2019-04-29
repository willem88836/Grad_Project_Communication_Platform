using Framework.ScriptableObjects.Variables;
using JsonUtility = Framework.Features.Json.JsonUtility;
using UnityEngine;

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

		udpMaster.UpdateTargetIP(ServerIP.Value);

		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);

		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountPhone.Value, "", AccountName.Value);
		SendMessage(connectMessage);
	}
	
	protected override void OnDestroy()
	{
		NetworkMessage disconnectMessage = new NetworkMessage(NetworkMessageType.DisconnectFromServer, ClientId);
		SendMessage(disconnectMessage);
		base.OnDestroy();
	}


	private void Start()
	{
		//udpMaster.UpdateTargetIP("145.37.144.87");
		//TransmitRoleplayDescription(new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", ClientId, JsonUtility.ToJson(new RoleplayDescription("", new Participant("Steve", "145.37.144.87", "123456"), new Participant("Stevette", "145.37.144.87", "123456"), new CaseDescription(new int[0], new int[0], RoleplayModule.Paraphrasing)))));
	}

	public void ConnectToServer(NetworkMessage message)
	{
		// TODO: store this somewhere properly when we have to switch between sending messages to server to client.
		string serverIP = message.SenderIP;
		udpMaster.UpdateTargetIP(serverIP);
	}

	public void TransmitRoleplayDescription(NetworkMessage message)
	{
		RoleplayDescription roleplayDescription = JsonUtility.FromJson<RoleplayDescription>(message.Message);

		bool isClient = roleplayDescription.Client.Id == ClientId;

		Participant other;
		Participant self; 

		if (isClient)
		{
			other = roleplayDescription.Professional;
			self = roleplayDescription.Client;
		}
		else
		{
			other = roleplayDescription.Client;
			self = roleplayDescription.Professional;
		}

		RoleplayCall.Initialize(isClient, other, self);

		ScreenController.SwitchScreenToModuleBriefing();
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
