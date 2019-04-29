using System;
using UnityEngine;
using UnityEngine.UI;

public class ModuleController : MonoBehaviour
{
	public ScreenController ScreenController;
	public GameObject ModuleButtonPrefab;
	public Transform ButtonContainer;
	public int textIndex;

	private NetworkClient networkClient;
	private RoleplayModule selectedModule = RoleplayModule.FreePlay;
	private bool inQueue = false;

	public void Initialize(NetworkClient networkClient)
	{
		this.networkClient = networkClient;
		CreateButtons();
	}

	private void CreateButtons()
	{
		Array modules = Enum.GetValues(typeof(RoleplayModule));
		foreach(RoleplayModule module in modules)
		{
			GameObject newButton = Instantiate(ModuleButtonPrefab, ButtonContainer);

			Text textField = newButton.transform.GetChild(textIndex).GetComponent<Text>();
			textField.text = module.ToString();

			Button button = newButton.GetComponent<Button>();
			button.onClick.AddListener(delegate { SelectModule(module); });
		}
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
