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
		base.Update();

		if (Input.GetKeyDown(KeyCode.R))
		{
			RoleplayDescription asdfff = new RoleplayDescription("1", new Participant("steve", "1.1.1.1", "123456789"), new Participant("Stevette", "1.1.1.1", "741852963"), new CaseDescription(new int[6][] { new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 }, new int[1] { 1 } }, RoleplayModule.Follow_Up_Questions));
			string asdfffString = JsonUtility.ToJson(asdfff);
			NetworkMessage asdf = new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", ClientId, asdfffString);

			TransmitRoleplayDescription(asdf);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			CompleteCaseEvaluation completeCaseEvaluation = new CompleteCaseEvaluation()
			{
				EvaluationUserA = new CaseEvaluation()
				{
					Id = "asdf",
					EvaluationFields = new string[2] { "something somehthing something", "more something something something" },
					User = new Participant("Name A", "1.1.1.1", "asdf")
				},
				EvaluationUserB = new CaseEvaluation()
				{
					Id = "asdf2",
					EvaluationFields = new string[2] { "something somehthing something", "more something something something" },
					User = new Participant("Name B", "1.1.1.1", "asdf")
				},
				RoleplayDescription = new RoleplayDescription()
				{
					Id = "1",
					UserA = new Participant("Name A", "1.1.1.1", "asdf"),
					UserB = new Participant("Name B", "1.1.1.1", "asdf"),
					Case = new CaseDescription()
					{
						Characteristics = new int[6][]
						{
							new int[1] {1},
							new int[1] {1},
							new int[1] {1},
							new int[1] {1},
							new int[1] {1},
							new int[1] {1}
						},
						Module = RoleplayModule.Follow_Up_Questions
					}
				}
			};

			TransmitCompleteEvaluation(new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", ClientId, JsonUtility.ToJson(completeCaseEvaluation)));
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


	public void ConnectToServer()
	{

	}

	[ExecuteOnMainThread]
	public void TransmitRoleplayDescription(NetworkMessage message)
	{
		ModuleController.LockOutModule();
		RoleplayController.OnRoleplayLoaded(message.Message);
	}

	[ExecuteOnMainThread]
	public void TransmitCompleteEvaluation(NetworkMessage message)
	{
		if (CompleteEvaluationController.WaitingForCompleteEvaluation)
		{
			CompleteEvaluationController.PrepareScreen(message.Message);
			ScreenController.SwitchScreenToCompleteEvaluation();
		}
	}
}
