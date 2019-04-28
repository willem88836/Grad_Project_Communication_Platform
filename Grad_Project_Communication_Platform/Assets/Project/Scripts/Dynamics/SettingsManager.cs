using System;
using UnityEngine;
using UnityEngine.UI;
using Framework.ScriptableObjects.Variables;


public class SettingsManager : MonoBehaviour
{
	public SettingsSliderInt[] intSliders;
	public SettingsSliderFloat[] floatSliders;

	private void Start()
	{
		for (int i = 0; i < intSliders.Length; i++)
		{
			SettingsSliderInt slider = intSliders[i];
			slider.Slider.value = slider.Value.Value;
			slider.Text.text = slider.Slider.value.ToString();
			slider.Slider.onValueChanged.AddListener(delegate { OnIntSliderValueChanged(slider); });
		}

		for (int i = 0; i < floatSliders.Length; i++)
		{
			SettingsSliderFloat slider = floatSliders[i];
			slider.Slider.value = slider.Value.Value;
			slider.Text.text = slider.Slider.value.ToString();
			slider.Slider.onValueChanged.AddListener(delegate { OnFloatSliderValueChanged(slider); });
		}
	}


	public void OnIntSliderValueChanged(SettingsSliderInt slider)
	{
		int v = (int)slider.Slider.value;
		slider.Value.Value = v;
		slider.Text.text = v.ToString();
	}

	public void OnFloatSliderValueChanged(SettingsSliderFloat slider)
	{
		float v = (float)slider.Slider.value;
		slider.Value.Value = (float)slider.Slider.value;
		slider.Text.text = v.ToString();
	}
}


[Serializable]
public struct SettingsSliderInt
{
	public Slider Slider;
	public Text Text;
	public SharedInt Value;
}

[Serializable]
public struct SettingsSliderFloat
{
	public Slider Slider;
	public Text Text;
	public SharedFloat Value;
}
