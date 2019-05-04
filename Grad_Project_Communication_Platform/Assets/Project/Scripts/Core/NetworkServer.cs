using System.Collections.Generic;

public sealed class NetworkServer : NetworkManager
{
	public Matchmaker Matchmaker;
	public CompleteEvaluationGenerator CompleteEvaluationGenerator;

	private List<Participant> activeUsers;


	protected override void Awake()
	{
		base.Awake();
		activeUsers = new List<Participant>();
		Matchmaker.Initialize(this);
		CompleteEvaluationGenerator.Initialize(this);
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


	private Participant SelectParticipant(NetworkMessage message)
	{
		return activeUsers.Find((Participant p) => p.Id == message.SenderId);
	}
}
