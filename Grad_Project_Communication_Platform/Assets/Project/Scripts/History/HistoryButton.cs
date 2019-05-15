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
		public Text IconField;

		private string id;

		public void Set(string log)
		{
			Debug.Log(log);

			// HACK: JSON is broken.
			string[] splittedDetails = log.Split(new char[1] { (char)124 });

			Debug.Log(splittedDetails.Length);

			Debug.Log(splittedDetails[0]);
			Debug.Log(splittedDetails[1]);
			Debug.Log(splittedDetails[2]);
			Debug.Log(splittedDetails[3]);
			Debug.Log(splittedDetails[4]);
			

			string module = splittedDetails[0];
			string nameUserA = splittedDetails[1];
			string nameUserB = splittedDetails[2];
			string date = splittedDetails[3];
			string id = splittedDetails[4];

			string key = string.Format("module_{0}", module);
			string moduleName = MultilanguageSupport.GetKeyWord(key);
			Header.text = moduleName;
			IconField.text = moduleName[0].ToString();
			SubHeader.text = string.Format("{0} and {1}", nameUserA, nameUserB);

			DateField.text = DateTime.Parse(date).ToShortDateString();

			this.id = id;
		}

		public void OnClick()
		{
			// do something With id.
			throw new NotImplementedException();
		}
	}
}
