using Framework.Utils;
using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class RoleplayController : MonoBehaviour
{
	public ProfileContainer ProfileContainer;

	[Space]
	public ScreenController ScreenController;
	public RoleplayCall RoleplayCall;
	public CompleteEvaluationController CompleteEvaluationController;

	[Header("Briefing Screen")]
	public Text ProfessionalName;
	public Text ClientName;
	public Transform UserACaseField;
	public Transform UserBCaseField;
	public VisualCaseElement ElementObject;


	[Header("EvaluationScreen")]
	public InputField[] ClientEvaluationFields;
	public InputField[] ProfessionalEvaluationFields;


	private NetworkClient networkClient;
	private RoleplayDescription currentRoleplay;
	private bool isClient; 

	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}


	public void OnRoleplayLoaded(string serializedRoleplayDescription)
	{
		currentRoleplay = JsonUtility.FromJson<RoleplayDescription>(serializedRoleplayDescription);

		isClient = currentRoleplay.UserA.Id == networkClient.ClientId;

		Participant other;
		Participant self;

		PrepareBriefingScreens(currentRoleplay);

		if (isClient)
		{
			other = currentRoleplay.UserB;
			self = currentRoleplay.UserA;
			ScreenController.SwitchScreenToClientBriefing();
		}
		else
		{
			other = currentRoleplay.UserA;
			self = currentRoleplay.UserB;
			ScreenController.SwitchScreenToProfessionalBriefing();
		}

		RoleplayCall.Initialize(isClient, other, self);
	}

	private void PrepareBriefingScreens(RoleplayDescription roleplay)
	{
		ModuleCaseProfile profile = ProfileContainer.GetCaseProfileOfModule(roleplay.Case.Module);

		UserBCaseField.ReversedForeach((Transform t) => { Destroy(t.gameObject); });
		UserACaseField.ReversedForeach((Transform t) => { Destroy(t.gameObject); });

		ClientName.text = roleplay.UserB.Name;
		ProfessionalName.text = roleplay.UserA.Name;

		for(int i = 0; i < roleplay.Case.Characteristics.Length; i++)
		{
			int[] characteristics = roleplay.Case.Characteristics[i];

			CaseElement element = profile.Elements[i];

			VisualCaseElement visualCaseUserA = null;
			VisualCaseElement visualCaseUserB = null;

			if (element.VisibleUserA)
			{
				visualCaseUserA = Instantiate(ElementObject, UserACaseField);
				visualCaseUserA.SetName(element.Name);
			}

			if (element.VisibleUserB)
			{
				visualCaseUserB = Instantiate(ElementObject, UserBCaseField);
				visualCaseUserB.SetName(element.Name);
			}

			for (int j = 0; j < characteristics.Length; j++)
			{
				string option = element.OptionPool[j];

				if (element.VisibleUserA)
				{
					visualCaseUserA.AddCharacteristic(option);
				}

				if (element.VisibleUserB)
				{
					visualCaseUserB.AddCharacteristic(option);
				}
			}
		}
	}


	public void FinishEvaluation()
	{
		SendEvaluation(isClient ? ClientEvaluationFields : ProfessionalEvaluationFields);
	}

	private void SendEvaluation(InputField[] inputFields)
	{
		// creates the case evaluation.
		CaseEvaluation caseEvaluation = new CaseEvaluation()
		{
			Id = currentRoleplay.Id,
			User = isClient ? currentRoleplay.UserA : currentRoleplay.UserB,
			EvaluationFields = new string[inputFields.Length]
		};

		for(int i = 0; i < inputFields.Length; i++)
		{
			caseEvaluation.EvaluationFields[i] = inputFields[i].text;
			inputFields[i].text = "";
		}

		// Sends the evaluation to the server.
		string json = JsonUtility.ToJson(caseEvaluation);
		NetworkMessage evaluationMessage = new NetworkMessage(NetworkMessageType.TransmitEvaluationTest, networkClient.ClientId, "", json);
		networkClient.SendMessage(evaluationMessage);

		CompleteEvaluationController.RequestCompleteEvaluation(caseEvaluation.Id);
		ScreenController.SwitchScreenToCompleteEvaluation();
	}

	public void ForceEndCall()
	{
		RoleplayCall.ForceEndCall();
	}
}
