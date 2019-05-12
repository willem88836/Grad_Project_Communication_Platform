using Framework.Features.Json;
using Framework.ScriptableObjects.Variables;
using Framework.Storage;

namespace Project.History
{
	public class HistoryManager : ApplicationController<NetworkServer>
	{
		public int ChunkSize = 5;
		public SharedString CompleteEvaluationName;


		public void OnHistoryLogsRequested(Participant participant, string serializedIndex)
		{
			int index = int.Parse(serializedIndex);

			SerializedHistory serializedHistory = new SerializedHistory()
			{
				Index = index,
				SerializedHistoryLogs = new string[ChunkSize]
			};

			// loads the available logs.
			for (int i = 0; i < ChunkSize; i++)
			{
				int j = i + index;

				string name = string.Format(CompleteEvaluationName.Value, j);
				SaveLoad.Load(name, out string log);

				if (log == null)
					break;

				serializedHistory.SerializedHistoryLogs[i] = log;
			}

			string json = JsonUtility.ToJson(serializedHistory);
			NetworkMessage message = new NetworkMessage(NetworkMessageType.RequestHistoryLogs, "", participant.Id, json);
			Manager.SendMessage(message, participant.IP);
		}
	}
}
