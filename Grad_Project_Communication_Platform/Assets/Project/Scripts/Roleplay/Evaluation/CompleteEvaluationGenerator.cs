using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

[Serializable]
public class CompleteEvaluationGenerator : ApplicationController<NetworkServer>
{
	public SharedString RoleplaySaveName;
	public SharedString CompleteEvaluationName;
	public SharedString UserLogsName;

	private Dictionary<string, CaseEvaluation> acquiredEvaluations = new Dictionary<string, CaseEvaluation>();


	public void SendCompleteEvaluation(string id, Participant participant)
	{
		string json;
		SaveLoad.Load(string.Format(CompleteEvaluationName.Value, id), out json);
		if (json != null)
		{
			NetworkMessage message = new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", participant.Id, json);
			Manager.SendMessage(message, participant.IP);
		}
	}

	public void OnEvaluationAcquired(string serializedEvaluation)
	{
		CaseEvaluation caseEvaluation = JsonUtility.FromJson<CaseEvaluation>(serializedEvaluation);

		string id = caseEvaluation.Id;
		if(acquiredEvaluations.ContainsKey(id))
		{
			GenerateCompleteEvaluation(id, caseEvaluation);
		}
		else
		{
			acquiredEvaluations.Add(id, caseEvaluation);
		}
	}

	private void GenerateCompleteEvaluation(string id, CaseEvaluation caseEvaluation)
	{
		// Removes the case from the dictionary.
		CaseEvaluation other = acquiredEvaluations[id];
		acquiredEvaluations.Remove(id);

		// Converts the stored roleplay to an object.
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
			EvaluationUserB = isUserA ? other : caseEvaluation,
			TimeStamp = DateTime.Now.ToString()
		};

		string completeEvalJson = JsonUtility.ToJson(completeCaseEvaluation);

		SaveLoad.Save(completeEvalJson, string.Format(CompleteEvaluationName.Value, id));

		SendMessageTo(roleplayDescription.UserA, completeEvalJson);
		SendMessageTo(roleplayDescription.UserB, completeEvalJson);

		AddIdToUserLog(roleplayDescription.UserA.Id, id);
		AddIdToUserLog(roleplayDescription.UserB.Id, id);

		SaveLoad.Remove(fileName);
	}

	private void AddIdToUserLog(string userId, string evaluationId)
	{
		SaveLoad.Append(string.Format(UserLogsName.Value, userId), string.Format("{0},", evaluationId));
	}

	private void SendMessageTo(Participant user, string message)
	{
		NetworkMessage completeEvalMessage = new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, "", user.Id, message);
		Manager.SendMessage(completeEvalMessage);
	}
}
