﻿using System.Collections.Generic;

public sealed class NetworkServer : NetworkManager
{
	private Matchmaker matchmaker;


	private List<Participant> activeUsers;


	protected override void Awake()
	{
		base.Awake();

		activeUsers = new List<Participant>();
		matchmaker = new Matchmaker();
		matchmaker.Initialize(this);
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