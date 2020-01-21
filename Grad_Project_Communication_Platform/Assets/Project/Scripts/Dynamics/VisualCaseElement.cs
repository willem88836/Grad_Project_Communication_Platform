using System;
using Framework.Language;
using UnityEngine;
using UnityEngine.UI;

public class VisualCaseElement : MonoBehaviour
{
	private Text Label;
	private Text Characteristic;

	private void Awake()
	{
		GetLabel();
		GetCharacteristic();
	}

	private void GetCharacteristic()
	{
		Characteristic = transform.GetChild(1).GetComponent<Text>();
		Characteristic.text = "";
	}

	private void GetLabel()
	{
		Label = transform.GetChild(0).GetComponent<Text>();
	}

	public void SetName(string name)
	{

		if (!Label)
			GetLabel();

		Label.text = MultilanguageSupport.GetKeyWord(name);
	}

	public void AddCharacteristic(string characteristic)
	{
		if (!Characteristic)
			GetCharacteristic();

		Text c = Instantiate(Characteristic, transform);
		c.gameObject.SetActive(true);
		c.text = MultilanguageSupport.GetKeyWord(characteristic);
	}
}
