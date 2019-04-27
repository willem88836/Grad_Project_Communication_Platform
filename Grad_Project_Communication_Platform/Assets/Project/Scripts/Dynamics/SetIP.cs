using Framework.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

public class SetIP : MonoBehaviour
{
	public InputField inputField;
	public SharedString sharedString;

	public void ApplyIP()
	{
		sharedString.Value = inputField.text;
	}
}
