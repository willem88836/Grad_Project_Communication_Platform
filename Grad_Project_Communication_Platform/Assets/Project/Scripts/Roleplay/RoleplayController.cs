using UnityEngine;

public class RoleplayController : MonoBehaviour
{
	public ScreenController ScreenController;

	private NetworkClient networkClient;

	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}


	public void OnRoleplayLoaded(RoleplayDescription roleplayDescription, bool isClient)
	{
		if (isClient)
		{
			ScreenController.SwitchScreenToClientBriefing();
		}
		else
		{
			ScreenController.SwitchScreenToProfessionalBriefing();
		}
	}
}
