using UnityEngine;

[CreateAssetMenu(menuName = "Roleplay/Case Element")]
public class CaseElement : ScriptableObject
{
	public string Name;
	public bool VisibleUserA;
	public bool VisibleUserB;
	public string[] OptionPool;
	public int OptionCount; 
}
