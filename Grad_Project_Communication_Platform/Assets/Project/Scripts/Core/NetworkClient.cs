using Framework.ScriptableObjects.Variables;
using Framework.Variables;
using UnityEngine;

public sealed class NetworkClient : NetworkManager
{
	[Space]
	[SerializeField] private SharedString AccountName;
	[SerializeField] private SharedString AccountPhone;

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

		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountName.Value, "", AccountName.Value);
		SendMessage(connectMessage);
	}

	private void Start()
	{
		TransmitRoleplayDescription(new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", ClientId, JsonUtility.ToJson(new RoleplayDescription("", new Participant("Steve", "1.1.1.1", "123456"), new Participant("Stevette", "1.1.1.1", "123456"), new CaseDescription(new int[0], new int[0], RoleplayModule.Paraphrasing)))));
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
