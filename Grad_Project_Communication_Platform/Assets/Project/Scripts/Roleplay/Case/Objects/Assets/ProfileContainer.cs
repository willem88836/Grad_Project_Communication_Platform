using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Roleplay/ProfileContainer")]
public class ProfileContainer : ScriptableObject
{
	public ModuleCaseProfile[] Profiles;

	public ModuleCaseProfile GetCaseProfileOfModule(RoleplayModule module)
	{
		IEnumerable<ModuleCaseProfile> profile = Profiles.Where(p => p.Module == module);

		if (profile.Any())
		{
			return profile.ToArray()[0];
		}

		return null;
	}
}
