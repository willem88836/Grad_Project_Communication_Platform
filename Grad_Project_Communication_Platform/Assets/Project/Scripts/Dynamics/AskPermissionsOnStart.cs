using UnityEngine;
using UnityEngine.Android;

public class AskPermissionsOnStart : MonoBehaviour
{
	private void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	private void Update()
	{
		if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
		{
			Permission.RequestUserPermission(Permission.Camera);
		}
		else if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
		{
			Permission.RequestUserPermission(Permission.Microphone);
		}
	}
}
