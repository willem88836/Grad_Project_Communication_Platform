using Project.History;
using Project.Social;
using System.Collections.Generic;

public sealed class NetworkServer  : NetworkManager
{
	public Matchmaker Matchmaker;
	public CompleteEvaluationGenerator CompleteEvaluationGenerator;
	public HistoryManager HistoryManager;
	public SocialManager SocialManager;

	private List<Participant> activeUsers;

	protected override void Update()
	{
		base.Update();

		if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C) && UnityEngine.Application.isEditor)
		{
			Participant userA = new Participant("TesterA", "1.1.1.1", "devm_9874123");
			Participant userB = new Participant("TesterB", "1.1.1.1", "devm_651089");

			ConnectToServer(new NetworkMessage(NetworkMessageType.ConnectToServer, userA.Id, "", userA.Name) { SenderIP = userA.IP });
			ConnectToServer(new NetworkMessage(NetworkMessageType.ConnectToServer, userB.Id, "", userB.Name) { SenderIP = userB.IP });

			Framework.Storage.SaveLoad.Load("LastId", out string id);

			RoleplayModule module = (RoleplayModule)UnityEngine.Random.Range(0, 1 + (int)RoleplayModule.open_questions);

			Enqueue(new NetworkMessage(NetworkMessageType.Enqueue, userA.Id, "", module.ToString()) { SenderIP = userA.IP });
			Enqueue(new NetworkMessage(NetworkMessageType.Enqueue, userB.Id, "", module.ToString()) { SenderIP = userB.IP });

			CaseEvaluation caseEvaluationA = new CaseEvaluation() { User = userA, Id = id, EvaluationFields = new string[2] { "", "" } };
			CaseEvaluation caseEvaluationB = new CaseEvaluation() { User = userB, Id = id, EvaluationFields = new string[2] { "", "" } };

			TransmitEvaluationTest(new NetworkMessage(NetworkMessageType.TransmitEvaluationTest, userA.Id, "", Framework.Features.Json.JsonUtility.ToJson(caseEvaluationA)) { SenderIP = userA.IP });
			TransmitEvaluationTest(new NetworkMessage(NetworkMessageType.TransmitEvaluationTest, userB.Id, "", Framework.Features.Json.JsonUtility.ToJson(caseEvaluationB)) { SenderIP = userB.IP });
		}
	}

	protected override void Initialize()
	{
		base.Initialize();

		activeUsers = new List<Participant>();

		Matchmaker.Initialize(this);
		CompleteEvaluationGenerator.Initialize(this);
		HistoryManager.Initialize(this);
		SocialManager.Initialize(this);
	}


	public void ConnectToServer(NetworkMessage message)
	{
		if (SelectParticipant(message) != null)
			return;

		Participant newUser = new Participant(message.Message, message.SenderIP, message.SenderId);
		activeUsers.Add(newUser);

		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, "", newUser.Id);
		SendMessage(connectMessage, message.SenderIP);
	}

	public void DisconnectFromServer(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);

		if (SelectParticipant(message) == null)
			return;

		activeUsers.Remove(participant);
	}

	public void Enqueue(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		Matchmaker.Enqueue(participant, message.Message);
	}

	public void Dequeue(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		Matchmaker.Dequeue(participant, message.Message);
	}

	public void TransmitEvaluationTest(NetworkMessage message)
	{
		CompleteEvaluationGenerator.OnEvaluationAcquired(message.Message);
	}

	public void TransmitCompleteEvaluation(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		CompleteEvaluationGenerator.SendCompleteEvaluation(message.Message, participant);
	}

	public void RequestHistoryLogs(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		HistoryManager.OnHistoryLogsRequested(participant, message.Message);
	}

	public void RequestSocialLogs(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		SocialManager.OnRequestAcquired(participant);
	}

	private Participant SelectParticipant(NetworkMessage message)
	{
		return activeUsers.Find((Participant p) => p.Id == message.SenderId);
	}
}
