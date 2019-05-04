using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class RoleplayController : MonoBehaviour
{
	public ProfileContainer ProfileContainer;

	[Space]
	public ScreenController ScreenController;
	public RoleplayCall RoleplayCall;

	[Header("Briefing Screen")]
	public Text ProfessionalName;
	public Text ClientName;
	public Text ProfessionalBriefing;
	public Text ClientBriefing;

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

		ClientBriefing.text = "";
		ProfessionalBriefing.text = "";

		ClientName.text = roleplay.UserB.Name;
		ProfessionalName.text = roleplay.UserA.Name;

		for(int i = 0; i < roleplay.Case.Characteristics.Length; i++)
		{
			int[] characteristics = roleplay.Case.Characteristics[i];

			CaseElement element = profile.Elements[i];

			for (int j = 0; j < characteristics.Length; j++)
			{
				string option = string.Format("{1}: {0}\n", element.OptionPool[j], element.Name);

				if (element.VisibleUserA)
				{
					ClientBriefing.text += option;
				}

				if (element.VisibleUserB)
				{
					ProfessionalBriefing.text += option;
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
	}

	public void ForceEndCall()
	{
		RoleplayCall.ForceEndCall();
	}
}
