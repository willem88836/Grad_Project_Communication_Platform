using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using UnityEngine;
using Framework.Utils;
using JsonUtility = Framework.Features.Json.JsonUtility;
using System.Collections.Generic;

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
				data = JsonUtility.ToJson(new SerializedSocial() { Friends = new Participant[0], Recent = new Participant[0] });
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
				string json = JsonUtility.ToJson(new SerializedSocial() {Recent = new Participant[0], Friends = new Participant[0]});
				SaveLoad.Save(json, name);
				AddToRecentLog(userA, userB);
				return;
			}

			SerializedSocial serializedSocial = JsonUtility.FromJson<SerializedSocial>(log);

			List<Participant> newRecent = new List<Participant>(serializedSocial.Recent);
			newRecent.Insert(0, userB);
			serializedSocial.Recent = newRecent.ToArray();

			string updatedLog = JsonUtility.ToJson(serializedSocial);
			SaveLoad.Save(updatedLog, name);
		}
	}
}
