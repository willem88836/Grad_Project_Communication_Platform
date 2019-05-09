using Framework.ScriptableObjects.Variables;
using Framework.Storage;
using System;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class RoleplayDescriptionGenerator
{
	public string CaseIdName = "LastId";
	public ProfileContainer ProfileContainer;

	private Random random = new Random();


	public RoleplayDescription Generate(Participant participantA, Participant participantB, RoleplayModule module)
	{
		CaseDescription caseDescription = GenerateCase(module);

		RoleplayDescription roleplayDescription;

		string id = GetCaseId();

		int rnd = random.Next(0, 2);
		if (rnd % 2 == 0)
		{
			roleplayDescription = new RoleplayDescription(id, participantA, participantB, caseDescription);
		}
		else
		{
			roleplayDescription = new RoleplayDescription(id, participantB, participantA, caseDescription);
		}

		return roleplayDescription;
	}

	private string GetCaseId()
	{
		if(!SaveLoad.Exists(CaseIdName))
		{
			SaveLoad.Save("0", CaseIdName);
		}

		string id;
		SaveLoad.Load(CaseIdName, out id);

		int idI = int.Parse(id) + 1;
		SaveLoad.Save(idI.ToString(), CaseIdName);

		return id;
	}

	private CaseDescription GenerateCase(RoleplayModule module)
	{
		ModuleCaseProfile profile = ProfileContainer.GetCaseProfileOfModule(module);

		int[][] elementData = new int[profile.Elements.Length][];

		for (int i = 0; i < profile.Elements.Length; i++)
		{
			CaseElement element = profile.Elements[i];

			int count = Mathf.Min(element.OptionCount, element.OptionPool.Length);
			int[] data = elementData[i] = new int[count];

			for (int j = 0; j < count; j++)
			{
				int k = random.Next(element.OptionPool.Length);

				for (int l = 0; l < j; l++)
				{
					int m = data[l];

					if (m == j)
					{
						j--;
						break;
					}
				}

				data[j] = k;
			}
		}

		CaseDescription caseDescription = new CaseDescription(elementData, module);
		return caseDescription;
	}
}
