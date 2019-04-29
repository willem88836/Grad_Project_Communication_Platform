using UnityEngine;

public class ModuleController : MonoBehaviour
{
	public ScreenController ScreenController;
	public GameObject ModuleButtonPrefab;
	public Transform ButtonContainer;

	private RoleplayModule selectedModule = RoleplayModule.FreePlay;
	private NetworkClient networkClient;
	private bool inQueue = false;


	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}


	public void SelectModule(RoleplayModule module)
	{
		if (inQueue)
			return;

		selectedModule = module;
		ScreenController.SwitchScreenToModuleBriefing();
	}

	public void LockInModule()
	{
		inQueue = true;
		NetworkMessage queueMessage = new NetworkMessage(NetworkMessageType.Enqueue, networkClient.ClientId, "", selectedModule.ToString());
		networkClient.SendMessage(queueMessage);
	}

	public void LockOutModule()
	{
		inQueue = false;
		NetworkMessage dequeueMessage = new NetworkMessage(NetworkMessageType.Dequeue, networkClient.ClientId, "", selectedModule.ToString());
		networkClient.SendMessage(dequeueMessage);
	}

	public void ToggleLockIn()
	{
		if (inQueue)
		{
			LockOutModule();
		}
		else
		{
			LockInModule();
		}
	}
}

// TODO: add all the modules. 
public enum RoleplayModule
{
	FreePlay,
	Paraphrasing, 
	Follow_Up_Questions,
	Open_Questions
};
