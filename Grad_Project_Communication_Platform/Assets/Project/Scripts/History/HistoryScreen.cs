using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

namespace Project.History
{
	public class HistoryScreen : ApplicationController<NetworkClient>
	{
		public Transform ButtonContainer;
		public HistoryButton ButtonPrefab;
		public Transform LoadMoreButton;

		private int currentIndex = 0;


		public override void Initialize(NetworkClient manager)
		{
			base.Initialize(manager);

			if (currentIndex != 0)
				return;

			LoadMoreButton.GetComponent<Button>().onClick.AddListener(RequestNextHistoryLogs);
			RequestNextHistoryLogs();
		}


		public void RequestHistoryLogs(int i)
		{
			NetworkMessage requestMessage = new NetworkMessage(NetworkMessageType.RequestHistoryLogs, Manager.ClientId, "", i.ToString());
			Manager.SendMessage(requestMessage);
		}

		public void RequestNextHistoryLogs()
		{
			RequestHistoryLogs(currentIndex);
			currentIndex++;
		}


		public void OnHistoryLogsAcquired(string serializedLogs)
		{
			SerializedHistory serializedHistory = JsonUtility.FromJson<SerializedHistory>(serializedLogs);

			for(int i = 0; i < serializedHistory.SerializedHistoryLogs.Length; i++)
			{
				string log = serializedHistory.SerializedHistoryLogs[i];

				if (log == null || log == "")
					break;

				HistoryButton button = Instantiate(ButtonPrefab, ButtonContainer);
				button.Set(log);
			}

			LoadMoreButton.SetAsLastSibling();
		}
	}
}