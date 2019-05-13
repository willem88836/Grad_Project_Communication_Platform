using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using UnityEngine;
using Framework.Utils;

namespace Project.Social
{
	public class SocialManager : ApplicationController<NetworkServer>
	{
		public SharedString UserSocialLogName;
		public int RecentCount;


		public void OnRequestAcquired(Participant participant)
		{
			string name = UserSocialLogName.Value.Format(participant.Id);
			SaveLoad.Load(name, out string data);

			if (data == null)
			{
				data = JsonUtility.ToJson(new SerializedSocial());
			}

			NetworkMessage message = new NetworkMessage(NetworkMessageType.RequestSocialLogs, "", participant.Id, data);
			Manager.SendMessage(message, participant.IP);
		}


		public void AddToRecentLog(Participant userA, Participant userB)
		{
			// HACK: This whole function is a bit of a hack. But it works for now :P 
			string name = UserSocialLogName.Value.Format(userA.Id);
			SaveLoad.Load(name, out string log);
			if (log == null)
			{
				string json = JsonUtility.ToJson(new SerializedSocial() {Recent = new Participant[RecentCount] });
				SaveLoad.Save(name, json);
				AddToRecentLog(userA, userB);
				return;
			}

			SerializedSocial serializedSocial = JsonUtility.FromJson<SerializedSocial>(log);

			for (int i = serializedSocial.Recent.Length - 2; i >= 0; i--)
			{
				serializedSocial.Recent[i + 1] = serializedSocial.Recent[i];
			}
			serializedSocial.Recent[0] = userB;

			string updatedLog = JsonUtility.ToJson(serializedSocial);
			SaveLoad.Save(updatedLog, name);
		}
	}
}
