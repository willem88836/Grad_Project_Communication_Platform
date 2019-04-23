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
	public Videocall Videocall;
	public RoleplayController RoleplayController;
	public ModuleController ModuleController;

	public string ClientId { get { return AccountPhone.Value; } } 
	public string ClientName { get { return AccountName.Value; } }


	protected override void Awake()
	{
		base.Awake();

		Videocall.Initialize(this);
		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);

		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountName.Value, "", AccountName.Value);
		SendMessage(connectMessage);
	}

	// FOO
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Videocall.StartCalling(true, new Participant("a", "1.1.1.1", "asdf"), new Participant("b", "1.1.1.1", "98765"));
		}
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

		Videocall.StartCalling(isClient, other, self); 
	}

	public void TransmitFinalEvaluation(NetworkMessage message)
	{

	}

	public void TransmitFootage(NetworkMessage message)
	{

	}

	public void ForceEndCall(NetworkMessage message)
	{
		Videocall.ForceEndCalling();

		// TODO: Switch to the right screen. 
		ScreenController.SwitchScreenToConversationChallengeTest();
	}

	public void ForceDisconnect(NetworkMessage message)
	{

	}
}
