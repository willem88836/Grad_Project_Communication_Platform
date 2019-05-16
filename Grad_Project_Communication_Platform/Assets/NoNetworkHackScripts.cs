using Framework.ScriptableObjects.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoNetworkHackScripts : MonoBehaviour
{
	public SharedString userName;
	public SharedString userPhone;
	public ScreenController ScreenController;
	public RoleplayDescriptionGenerator RoleplayDescriptionGenerator;
	public RoleplayController RoleplayController;
	[Space]
	public InputField[] ClientFields;
	public InputField[] ProfessionalFields;
	public CompleteEvaluationController CompleteEvaluationController;


	private RoleplayDescription currentRoleplayDescription;


	private bool isClient;


	public void LockInModule()
	{
		StartCoroutine(QueueSim());
	}

	private IEnumerator<YieldInstruction> QueueSim()
	{
		yield return new WaitForSeconds(5);

		isClient = userPhone.Value == "devm_9874123";

		Participant user1 = new Participant(userName.Value, "1.1.1.1", userPhone.Value);
		Participant user2 = new Participant("TesterA", "1.1.1.1", "devm_651089");



		RoleplayDescription roleplayDescription = new RoleplayDescription(
			"1",
			isClient ? user1 : user2,
			isClient ? user2 : user1,
			RoleplayDescriptionGenerator.GenerateCase(RoleplayModule.open_questions));
		currentRoleplayDescription = roleplayDescription;
		string json = Framework.Features.Json.JsonUtility.ToJson(roleplayDescription);
		RoleplayController.OnRoleplayLoaded(json);


		if (isClient)
		{
			ScreenController.SwitchScreenToClientBriefing();
		}
		else
		{
			ScreenController.SwitchScreenToProfessionalBriefing();
		}
	}





	public void FinishEvaluation()
	{
		ScreenController.SwitchScreenToCompleteEvaluationLoading();

		StartCoroutine(EvaluationLoadingSim());
	}

	private IEnumerator<YieldInstruction> EvaluationLoadingSim()
	{
		yield return new WaitForSeconds(5);

		CaseEvaluation clientEval = new CaseEvaluation()
		{
			Id = currentRoleplayDescription.Id,
			User = currentRoleplayDescription.UserA,
			EvaluationFields = new string[2]
			{
				ClientFields[0].text,
				ClientFields[1].text
			}
		};

		CaseEvaluation profEval = new CaseEvaluation()
		{
			Id = currentRoleplayDescription.Id,
			User = currentRoleplayDescription.UserB,
			EvaluationFields = new string[2]
			{
				ProfessionalFields[0].text,
				ProfessionalFields[1].text
			}
		};


		CompleteCaseEvaluation completeCaseEvaluation = new CompleteCaseEvaluation()
		{
			RoleplayDescription = currentRoleplayDescription,
			EvaluationUserA = clientEval,
			EvaluationUserB = profEval,
			TimeStamp = DateTime.Now.ToString()
		};

		string json = Framework.Features.Json.JsonUtility.ToJson(completeCaseEvaluation);

		CompleteEvaluationController.PrepareScreen(json);
		ScreenController.SwitchScreenToCompleteEvaluation();
	}
}
