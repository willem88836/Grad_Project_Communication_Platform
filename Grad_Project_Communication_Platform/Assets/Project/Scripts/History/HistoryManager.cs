using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using Framework.Utils;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;

namespace Project.History
{
	public class HistoryManager : ApplicationController<NetworkServer>
	{
		public int ChunkSize = 5;
		public SharedString CompleteEvaluationName;
		public SharedString HistoryLogsName;


		public void OnHistoryLogsRequested(Participant participant, string serializedIndex)
		{
			int index = int.Parse(serializedIndex);

			string[] ids = LoadUserLogs(participant.Id);


			int count = Mathf.Clamp(Mathf.Min(ChunkSize, ids.Length - index), 0, int.MaxValue);

			SerializedHistory serializedHistory = new SerializedHistory()
			{
				Index = index,
				SerializedHistoryLogs = new string[count]
			};

			for (int i = index; i < count; i++)
			{
				string id = ids[i];
				string name = string.Format(CompleteEvaluationName.Value, id);
				SaveLoad.Load(name, out string log);

				if (log == null)
				{
					Debug.LogWarning("Missing Evaluation Entry: " + id);
					break;
				}
				
				serializedHistory.SerializedHistoryLogs[i - index] = log;
			}

			string json = JsonUtility.ToJson(serializedHistory);
			NetworkMessage message = new NetworkMessage(NetworkMessageType.RequestHistoryLogs, "", participant.Id, json);
			Manager.SendMessage(message, participant.IP);
		}


		private string[] LoadUserLogs(string userId)
		{
			Debug.Log("asdf1: " + userId);
			string name = HistoryLogsName.Value.Format(userId);
			Debug.Log("asdf2: " + name);
			SaveLoad.Load(name, out string data);
			Debug.Log("asdf3: " + data);

			if (data == null)
				return new string[0];

			string[] indices = data.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
			return indices;
		}
	}
}
