using System;

public class RoleplayDescriptionGenerator
{
	private Random random = new Random();

	internal RoleplayDescription Generate(Participant participantA, Participant participantB, RoleplayModule module)
	{
		//TODO: Add proper case generation.
		CaseDescription caseDescription = new CaseDescription(new int[] { }, new int[] { }, module);

		RoleplayDescription roleplayDescription;

		// TODO: Add proper Id generation.
		string id = "";

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
