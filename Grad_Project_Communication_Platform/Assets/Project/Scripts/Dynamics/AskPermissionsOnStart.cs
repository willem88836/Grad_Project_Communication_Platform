using UnityEngine;
using UnityEngine.Android;

public class AskPermissionsOnStart : MonoBehaviour
{
	private void Start()
	{
		Permission.RequestUserPermission(Permission.Camera);
		Permission.RequestUserPermission(Permission.Microphone);
	}
}
