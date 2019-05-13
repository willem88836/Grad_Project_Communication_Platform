using Framework.Utils;
using UnityEngine;
using JsonUtility = Framework.Features.Json.JsonUtility;


namespace Project.Social
{
	public class SocialScreen : ApplicationController<NetworkClient>
	{
		public SocialButton ButtonPrefab;
		public Transform FriendsContainer;
		public Transform RecentContainer;


		public override void Initialize(NetworkClient manager)
		{
			base.Initialize(manager);

			NetworkMessage message = new NetworkMessage(NetworkMessageType.RequestSocialLogs, Manager.ClientId);
			Manager.SendMessage(message);
		}

		public void OnSocialAcquired(string serializedSocial)
		{
			SerializedSocial social = JsonUtility.FromJson<SerializedSocial>(serializedSocial);

			Spawn(FriendsContainer, social.Friends);
			Spawn(RecentContainer, social.Recent);
		}

		private void Spawn(Transform container, Participant[] participants)
		{
			container.DestroyAllChildren();

			foreach (Participant participant in participants)
			{
				SocialButton socialButton = Instantiate(ButtonPrefab, container);
				socialButton.Set(participant);
			}
		}
	}
}
