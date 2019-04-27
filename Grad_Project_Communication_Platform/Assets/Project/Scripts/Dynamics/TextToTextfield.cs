using UnityEngine;
using UnityEngine.UI;

public class TextToTextfield : MonoBehaviour
{
	public TextAsset TextFile;
	public Text TextField;

	public void Start()
	{
		TextField.text = TextFile.text;
		Debug.Log(TextFile.text);
	}
}
