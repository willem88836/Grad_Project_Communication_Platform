using UnityEngine;

[CreateAssetMenu(menuName = "Roleplay/CaseProfile")]
public class ModuleCaseProfile : ScriptableObject
{
	public RoleplayModule Module;
	public CaseElement[] Elements;

	public CaseElement GetElement(int i)
	{
		return Elements[i];
	}
}
