using Framework.ScriptableObjects.Variables;
using System;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class RoleplayDescriptionGenerator
{
	public SharedInt LastCaseId;
	public ProfileContainer ProfileContainer;

	private Random random = new Random();


	public RoleplayDescription Generate(Participant participantA, Participant participantB, RoleplayModule module)
	{
		CaseDescription caseDescription = GenerateCase(module);

		RoleplayDescription roleplayDescription;

		string id = LastCaseId.Value.ToString();
		LastCaseId.Value++;

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
