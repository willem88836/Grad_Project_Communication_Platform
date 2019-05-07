using Framework.ScriptableObjects.Variables;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

public sealed class NetworkClient : NetworkManager
{
	[Header("Data")]
	[SerializeField] private SharedString AccountName;
	[SerializeField] private SharedString AccountPhone;
	[SerializeField] private SharedString ServerIP;

	[Header("Controllers")]
	public ScreenController ScreenController;
	public RoleplayController RoleplayController;
	public ModuleController ModuleController;
	public CompleteEvaluationController CompleteEvaluationController;

	public string ClientId { get { return AccountPhone.Value; } } 
	public string ClientName { get { return AccountName.Value; } }


	protected override void Awake()
	{
		base.Awake();

		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);
		CompleteEvaluationController.Initialize(this);
	}

	//private void Start()
	//{
	//	string json;
	//	Framework.Storage.SaveLoad.Load("Complete_Evaluation_0", out json);
	//	TransmitCompleteEvaluation(new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", ClientId, json));
	//}

	protected override void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RoleplayDescription asdfff = new RoleplayDescription("1", new Participant("steve", "1.1.1.1", "123456789"), new Participant("Stevette", "1.1.1.1", "741852963"), new CaseDescription(new int[6][] { new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 } }, RoleplayModule.Follow_Up_Questions));
			string asdfffString = JsonUtility.ToJson(asdfff);
			NetworkMessage asdf = new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", ClientId, asdfffString);

			TransmitRoleplayDescription(asdf);
		}
	}

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

	[ExecuteOnMainThread]
	public void TransmitCompleteEvaluation(NetworkMessage message)
	{
		CompleteEvaluationController.PrepareScreen(message.Message);
	}
}
