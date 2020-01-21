using UnityEngine;

public class ModuleController : ApplicationController<NetworkClient>
{
	// TODO: generate the buttons and load the proper images instead of hard placing them.
	public ScreenController ScreenController;
	public Transform ButtonContainer;
	public ModuleBriefingPanel ModuleBriefingPanel;

	private RoleplayModule selectedModule = RoleplayModule.free_play;
	private bool inQueue = false;


	public void SelectModule(RoleplayModule module)
	{
		if (inQueue)
			return;

		selectedModule = module;
		ScreenController.SwitchScreenToModuleBriefing();
		ModuleBriefingPanel.Prepare(selectedModule);
	}

	public void LockInModule()
	{
		inQueue = true;
		NetworkMessage queueMessage = new NetworkMessage(NetworkMessageType.Enqueue, Manager.ClientId, "", selectedModule.ToString());
		Manager.SendMessage(queueMessage);
	}

	public void LockOutModule()
	{
		inQueue = false;
		NetworkMessage dequeueMessage = new NetworkMessage(NetworkMessageType.Dequeue, Manager.ClientId, "", selectedModule.ToString());
		Manager.SendMessage(dequeueMessage);
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
	free_play,
	paraphrasing, 
	follow_up_questions,
	open_questions
};
