using System;
using System.Collections.Generic;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class Matchmaker
{
	private NetworkServer networkServer;

	private Dictionary<RoleplayModule, List<Participant>> queues;
	private RoleplayDescriptionGenerator roleplayDescriptionGenerator;


	public void Initialize(NetworkServer networkServer)
	{
		this.networkServer = networkServer;

		roleplayDescriptionGenerator = new RoleplayDescriptionGenerator();

		queues = new Dictionary<RoleplayModule, List<Participant>>();
		Array modules = Enum.GetValues(typeof(RoleplayModule));
		foreach(RoleplayModule module in modules)
		{
			queues.Add(module, new List<Participant>());
		}
	}


	public void Enqueue(Participant participant, string serializedModule)
	{
		RoleplayModule module = (RoleplayModule)Enum.Parse(typeof(RoleplayModule), serializedModule);

		List<Participant> queue = queues[module];
		queue.Add(participant);
		FindMatch(module);
	}

	public void Dequeue(Participant participant, RoleplayModule module)
	{
		List<Participant> queue = queues[module];
		queue.Remove(participant);
	}


	private void FindMatch(RoleplayModule module)
	{
		List<Participant> queue = queues[module];
		if (queue.Count >= 2)
		{
			Participant participantA = queue[queue.Count - 1];
			Participant participantB = queue[queue.Count - 2];

			queue.RemoveAt(queue.Count - 1);
			queue.RemoveAt(queue.Count - 1);

			OnMatchFound(participantA, participantB, module);
		}
	}

	private void OnMatchFound(Participant participantA, Participant participantB, RoleplayModule module)
	{
		RoleplayDescription roleplayDescription = roleplayDescriptionGenerator.Generate(participantA, participantB, module);
		string json = JsonUtility.ToJson(roleplayDescription);

		SendRoleplayDescription(json, participantA);
		SendRoleplayDescription(json, participantB);
	}

	private void SendRoleplayDescription(string serializedRoleplayDescription, Participant receiver)
	{
		NetworkMessage networkMessage = new NetworkMessage(NetworkMessageType.TransmitRoleplayDescription, "", receiver.Id, serializedRoleplayDescription);
		networkServer.SendMessage(networkMessage);
	}
}
