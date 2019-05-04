using Framework.Language;
using UnityEngine;
using UnityEngine.UI;

public class VisualCaseElement : MonoBehaviour
{
	private Text Label;
	private Text Characteristic;

	private void Awake()
	{
		Label = transform.GetChild(0).GetComponent<Text>();
		Characteristic = transform.GetChild(1).GetComponent<Text>();
		Characteristic.text = "";
	}


	public void SetName(string name)
	{
		Label.text = MultilanguageSupport.GetKeyWord(name);
	}

	public void AddCharacteristic(string characteristic)
	{
		Text c = Instantiate(Characteristic, transform);
		c.gameObject.SetActive(true);
		c.text = MultilanguageSupport.GetKeyWord(characteristic);
	}
}
