public struct CaseDescription
{
	public int[] ClientCharacteristics;
	public int[] Assignments;
	public RoleplayModule Module;

	public CaseDescription(int[] clientCharacteristics, int[] assignments, RoleplayModule module)
	{
		this.ClientCharacteristics = clientCharacteristics;
		this.Assignments = assignments;
		this.Module = module;
	}
}
