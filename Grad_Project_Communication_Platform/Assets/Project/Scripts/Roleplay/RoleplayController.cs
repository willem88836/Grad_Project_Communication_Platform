using UnityEngine;
using UnityEngine.UI;
using JsonUtility = Framework.Features.Json.JsonUtility;

public class RoleplayController : MonoBehaviour
{
	public ProfileContainer ProfileContainer;

	[Space]
	public ScreenController ScreenController;
	public RoleplayCall RoleplayCall;

	[Space]
	public Text ProfessionalName;
	public Text ClientName;

	[Space]
	public Text ProfessionalBriefing;
	public Text ClientBriefing;


	private NetworkClient networkClient;


	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}

	public void OnRoleplayLoaded(string serializedRoleplayDescription)
	{
		RoleplayDescription roleplay = JsonUtility.FromJson<RoleplayDescription>(serializedRoleplayDescription);

		bool isClient = roleplay.UserA.Id == networkClient.ClientId;

		Participant other;
		Participant self;

		PrepareBriefingScreens(roleplay);

		if (isClient)
		{
			other = roleplay.UserB;
			self = roleplay.UserA;
			ScreenController.SwitchScreenToClientBriefing();
		}
		else
		{
			other = roleplay.UserA;
			self = roleplay.UserB;
			ScreenController.SwitchScreenToProfessionalBriefing();
		}

		RoleplayCall.Initialize(isClient, other, self);
	}

	private void PrepareBriefingScreens(RoleplayDescription roleplay)
	{
		ModuleCaseProfile profile = ProfileContainer.GetCaseProfileOfModule(roleplay.Case.Module);

		ClientBriefing.text = "";
		ProfessionalBriefing.text = "";

		ClientName.text = roleplay.UserB.Name;
		ProfessionalName.text = roleplay.UserA.Name;

		for(int i = 0; i < roleplay.Case.Characteristics.Length; i++)
		{
			int[] characteristics = roleplay.Case.Characteristics[i];

			CaseElement element = profile.Elements[i];

			for (int j = 0; j < characteristics.Length; j++)
			{
				string option = string.Format("{1}: {0}\n", element.OptionPool[j], element.Name);

				if (element.VisibleUserA)
				{
					ClientBriefing.text += option;
				}

				if (element.VisibleUserB)
				{
					ProfessionalBriefing.text += option;
				}
			}
		}
	}

	public void ForceEndCall()
	{
		RoleplayCall.ForceEndCall();
	}
}
