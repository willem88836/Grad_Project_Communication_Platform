using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class CompleteEvaluationController : MonoBehaviour
{
	[Header("Scene References")]
	public Text ModuleField;
	public Text UserANameField;
	public Text UserBNameField;
	public Transform CaseField;
	public Text[] EvaluationUserAFields;
	public Text[] EvaluationUserBFields;

	[Header("Assets")]
	public ProfileContainer ProfileContainer;
	public VisualCaseElement ElementObject;

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

		SetNames(caseEvaluation);
		SetElements(caseEvaluation);
		SetEvaluation(caseEvaluation.EvaluationUserA, EvaluationUserAFields);
		SetEvaluation(caseEvaluation.EvaluationUserB, EvaluationUserBFields);
	}

	private void SetNames(CompleteCaseEvaluation caseEvaluation)
	{
		UserANameField.text = caseEvaluation.RoleplayDescription.UserA.Name;
		UserBNameField.text = caseEvaluation.RoleplayDescription.UserB.Name;
		ModuleField.text = caseEvaluation.RoleplayDescription.Case.Module.ToString();
	}

	private void SetElements(CompleteCaseEvaluation caseEvaluation)
	{
		CaseDescription caseDescription = caseEvaluation.RoleplayDescription.Case;
		ModuleCaseProfile profile = ProfileContainer.GetCaseProfileOfModule(caseDescription.Module);

		int[][] characteristics = caseDescription.Characteristics;

		for(int i = 0; i < characteristics.Length; i++)
		{
			CaseElement element = profile.GetElement(i);
			VisualCaseElement visualCaseElement = Instantiate(ElementObject, CaseField);
			visualCaseElement.SetName(element.Name);
			int[] elementIndices = characteristics[i];

			for (int j = 0; j < elementIndices.Length; j++)
			{
				int k = elementIndices[j];
				string characteristic = element.OptionPool[k];

				visualCaseElement.AddCharacteristic(characteristic);
			}
		}
	}

	private void SetEvaluation(CaseEvaluation evaluation, Text[] Fields)
	{
		for (int i = 0; i < evaluation.EvaluationFields.Length; i++)
		{
			Fields[i].text = evaluation.EvaluationFields[i];
		}
	}
}
