using Framework.ScriptableObjects.Variables;
using System;

[Serializable]
public class RoleplayDescriptionGenerator
{
	public SharedInt LastCaseId; 

	private Random random = new Random();

	internal RoleplayDescription Generate(Participant participantA, Participant participantB, RoleplayModule module)
	{
		//TODO: Add proper case generation.
		CaseDescription caseDescription = new CaseDescription(new int[] { }, new int[] { }, module);

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
}
