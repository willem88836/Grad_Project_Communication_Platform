public class RoleplayDescription
{
	public string Id;
	public Participant UserA;
	public Participant UserB;
	public CaseDescription Case;

	public RoleplayDescription() { }

	public RoleplayDescription(string id, Participant userA, Participant userB, CaseDescription caseDescription)
	{
		this.Id = id;
		this.UserA = userA;
		this.UserB = userB;
		this.Case = caseDescription;
	}
}
