using Framework.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

public class SetIP : MonoBehaviour
{
	public InputField InputField;
	public SharedString SharedString;


	private void Start()
	{
		InputField.text = SharedString.Value;
	}

	public void ApplyIP()
	{
		SharedString.Value = InputField.text;
	}
}
