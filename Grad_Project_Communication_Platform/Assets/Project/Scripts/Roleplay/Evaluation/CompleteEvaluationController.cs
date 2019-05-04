using Framework.Features.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class CompleteEvaluationController : MonoBehaviour
{
	private NetworkClient networkClient;


	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}

	public void RequestCompleteEvaluation(string id)
	{
		NetworkMessage requestCompleteEvaluation = new NetworkMessage(NetworkMessageType.TransmitCompleteEvaluation, networkClient.ClientId);
		networkClient.SendMessage(requestCompleteEvaluation);
	}


	public void PrepareScreen(string serializedCompleteEvaluation)
	{
		CompleteCaseEvaluation caseEvaluation = JsonUtility.FromJson<CompleteCaseEvaluation>(serializedCompleteEvaluation);


	}
}
