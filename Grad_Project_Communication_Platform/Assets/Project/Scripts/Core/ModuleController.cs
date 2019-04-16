using UnityEngine;

public class ModuleController : MonoBehaviour
{
	private NetworkClient networkClient;
	private RoleplayModule selectedModule = RoleplayModule.None;
	private bool inQueue = false;

	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
	}

	public void SelectModule(RoleplayModule module)
	{
		if (!inQueue)
			selectedModule = module;
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
}

// TODO: add all the modules. 
public enum RoleplayModule
{
	None,
};
