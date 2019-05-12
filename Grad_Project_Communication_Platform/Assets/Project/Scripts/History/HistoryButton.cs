using Framework.Language;
using System;
using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

namespace Project.History
{
	public class HistoryButton : MonoBehaviour
	{
		public Text Header;
		public Text SubHeader;
		public Text DateField;
		

		public void Set(string log)
		{
			CompleteCaseEvaluation caseEvaluation = JsonUtility.FromJson<CompleteCaseEvaluation>(log);

			string key = string.Format("module_{0}", caseEvaluation.RoleplayDescription.Case.Module.ToString());
			Header.text = MultilanguageSupport.GetKeyWord(key);

			SubHeader.text = string.Format("{0} and {1}",
				caseEvaluation.EvaluationUserA.User.Name,
				caseEvaluation.EvaluationUserB.User.Name);

			DateField.text = DateTime.Parse(caseEvaluation.TimeStamp).ToShortDateString();
		}

		public void OnClick()
		{
			throw new NotImplementedException();
		}
	}
}
