using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class NetworkServer : NetworkManager
{
	private Matchmaker matchmaker;


	private List<Participant> activeUsers;


	protected override void Awake()
	{
		base.Awake();

		activeUsers = new List<Participant>();
		matchmaker = new Matchmaker();

		NetworkMessage msg = new NetworkMessage(NetworkMessageType.Enqueue, "", "", "");
		Enqueue(msg);
	}


	public void ConnectToServer(NetworkMessage message)
	{
		// TODO: Double check if sender iP is actually set.
		// TODO: Implement username.
		Participant newUser = new Participant("", message.SenderIP, message.Message);
		activeUsers.Add(newUser);
	}

	public void DisconnectFromServer(NetworkMessage message)
	{
		Participant participant = activeUsers.Find((Participant p) => p.IP == message.SenderIP);
		activeUsers.Remove(participant);
	}

	public void Enqueue(NetworkMessage message)
	{
		Participant participant = activeUsers.Find((Participant p) => p.IP == message.SenderIP);
		matchmaker.Enqueue(participant, message.Message);
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
}
