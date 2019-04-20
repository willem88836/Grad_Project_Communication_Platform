using System.Collections.Generic;
using UnityEngine;

public sealed class NetworkServer : NetworkManager
{
	[SerializeField] private Matchmaker matchmaker;


	private List<Participant> activeUsers;


	protected override void Awake()
	{
		base.Awake();

		activeUsers = new List<Participant>();
		matchmaker.Initialize(this);




		OnMessageReceived(new NetworkMessage(NetworkMessageType.ConnectToServer, "123456789", "", "Steve") { SenderIP = "145.37.144.11"});
		OnMessageReceived(new NetworkMessage(NetworkMessageType.ConnectToServer, "123456788", "", "Stevette") { SenderIP = "145.37.144.12" });

		OnMessageReceived(new NetworkMessage(NetworkMessageType.Enqueue, "123456789", "", "Paraphrasing") { SenderIP = "145.37.144.11" });
		OnMessageReceived(new NetworkMessage(NetworkMessageType.Enqueue, "123456788", "", "Paraphrasing") { SenderIP = "145.37.144.12" });
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
		matchmaker.Enqueue(participant, message.Message);
	}

	public void Dequeue(NetworkMessage message)
	{
		Participant participant = SelectParticipant(message);
		matchmaker.Dequeue(participant, message.Message);
	}

	public void StoreFootage(NetworkMessage message)
	{

	}

	public void TransmitEvaluationTest(NetworkMessage message)
	{

	}

	public void RemoveConnection(NetworkMessage message)
	{

	}



	private Participant SelectParticipant(NetworkMessage message)
	{
		return activeUsers.Find((Participant p) => p.IP == message.SenderIP);
	}
}
