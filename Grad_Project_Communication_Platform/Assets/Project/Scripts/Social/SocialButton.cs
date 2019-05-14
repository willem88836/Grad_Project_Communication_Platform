using Framework.Language;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Social
{
	public class SocialButton : MonoBehaviour
	{
		private const string online = "online";
		private const string offline = "offline";

		public Text HeaderField;
		public Text SubHeader;

		public Button CallButton;
		public Button HistoryButton;


		public void Set(Participant participant)
		{
			HeaderField.text = participant.Name; 
			SubHeader.text = MultilanguageSupport.GetKeyWord(participant.IP == "" ? offline : online);
		}


		public void OnCallButtonClicked()
		{
			throw new NotImplementedException();
		}

		public void OnHistoryButtonClicked()
		{
			throw new NotImplementedException();
		}
	}
}
