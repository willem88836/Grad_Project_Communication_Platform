using UnityEngine;

public class ModuleButton : MonoBehaviour
{
	public ModuleController ModuleController;

	public RoleplayModule RoleplayModule;

	public void OnClick()
	{
		ModuleController.SelectModule(RoleplayModule);
	}
}
