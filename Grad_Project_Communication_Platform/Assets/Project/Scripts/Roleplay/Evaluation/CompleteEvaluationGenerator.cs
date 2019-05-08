using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

[Serializable]
public class CompleteEvaluationGenerator
{
	public SharedString RoleplaySaveName;
	public SharedString CompleteEvaluationName;

	private NetworkServer networkServer;

	private Dictionary<string, CaseEvaluation> acquiredEvaluations = new Dictionary<string, CaseEvaluation>();


	public void Initialize(NetworkServer networkServer)
	{
		this.networkServer = networkServer;
	}


	public void SendCompleteEvaluation(string id, Participant participant)
	{
		string json;
		SaveLoad.Load(string.Format(CompleteEvaluationName.Value, id), out json);
		if (json != null)
		{
			NetworkMessage message = new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", participant.Id, json);
			networkServer.SendMessage(message, participant.IP);
		}
	}

	public void OnEvaluationAcquired(string serializedEvaluation)
	{
		CaseEvaluation caseEvaluation = JsonUtility.FromJson<CaseEvaluation>(serializedEvaluation);

		string id = caseEvaluation.Id;
		if(acquiredEvaluations.ContainsKey(id))
		{
			// Removes the case from the dictionary.
			CaseEvaluation other = acquiredEvaluations[id];
			acquiredEvaluations.Remove(id);

			// converts the stored roleplay to an object.
			string roleplayJson;
			string fileName = string.Format(RoleplaySaveName.Value, id);
			SaveLoad.Load(fileName, out roleplayJson);
			RoleplayDescription roleplayDescription = JsonUtility.FromJson<RoleplayDescription>(roleplayJson);

			// serialized the complete evaluation.
			bool isUserA = id == roleplayDescription.UserA.Id;
			CompleteCaseEvaluation completeCaseEvaluation = new CompleteCaseEvaluation()
			{
				RoleplayDescription = roleplayDescription,
				EvaluationUserA = isUserA ? caseEvaluation : other,
				EvaluationUserB = isUserA ? other : caseEvaluation
			};

			string completeEvalJson = JsonUtility.ToJson(completeCaseEvaluation);

			SaveLoad.Save(completeEvalJson, string.Format(CompleteEvaluationName.Value, id));

			// Sends the complete evaluation t othe users. 
			new Thread(new ThreadStart(delegate
			{
				SendMessageTo(roleplayDescription.UserA, completeEvalJson);
				SendMessageTo(roleplayDescription.UserB, completeEvalJson);

			})).Start();

			SaveLoad.Remove(fileName);
		}
		else
		{
			acquiredEvaluations.Add(id, caseEvaluation);
		}
	}

	private void SendMessageTo(Participant user, string message)
	{
		NetworkMessage completeEvalMessage = new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", user.Id, message);
		networkServer.SendMessage(completeEvalMessage);
	}
}
