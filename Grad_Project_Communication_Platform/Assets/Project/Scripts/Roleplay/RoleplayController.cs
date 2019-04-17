using UnityEngine;

public class RoleplayController : MonoBehaviour
{
	private NetworkClient networkClient;

	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}


}
