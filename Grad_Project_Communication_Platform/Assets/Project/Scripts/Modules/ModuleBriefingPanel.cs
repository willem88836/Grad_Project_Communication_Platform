using Framework.Language;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleBriefingPanel : MonoBehaviour
{
	public Text DescriptionField;
	public Text ExampleField;
	public Text ModuleName;
	public float SwitchTimer = 4;

	public void Prepare(RoleplayModule module)
	{
		string serializedModule = module.ToString();
		DescriptionField.text = MultilanguageSupport.GetKeyWord(string.Format("module_description_{0}", serializedModule));

		ModuleName.text = MultilanguageSupport.GetKeyWord(string.Format("module_{0}", serializedModule));

		StopAllCoroutines();
		StartCoroutine(SwitchExamples(serializedModule));
	}



	private IEnumerator<YieldInstruction> SwitchExamples(string module)
	{
		int i = 0;
		while (true)
		{
			string key = string.Format("module_example_{0}_{1}", module, i);
			string txt = MultilanguageSupport.GetKeyWord(key);
			if (txt == key)
			{
				i = 0;
			}
			else
			{
				ExampleField.text = txt;
				i++;
				yield return new WaitForSeconds(SwitchTimer);
			}
		}
	}
}
