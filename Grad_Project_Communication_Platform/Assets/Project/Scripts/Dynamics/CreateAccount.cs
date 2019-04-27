using UnityEngine;
using Framework.ScriptableObjects.Variables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateAccount : MonoBehaviour
{
	public string TargetScene;

	public SharedString AccountName;
	public SharedString AccountPhone;

	public Text NameField;
	public Text PhoneField;


	public void Finish()
	{
		// TODO: do some proper name checking here. 
		if (NameField.text != "" && PhoneField.text != "") 
		{
			AccountName.Value = NameField.text;
			AccountPhone.Value = PhoneField.text;

			SceneManager.LoadScene(TargetScene);
		}
	}
}
