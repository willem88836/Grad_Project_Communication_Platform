public struct RoleplayDescription
{
	public string Id;
	public Participant Client;
	public Participant Professional;
	public CaseDescription Case;

	public RoleplayDescription(string id, Participant client, Participant professional, CaseDescription @case)
	{
		this.Id = id;
		this.Client = client;
		this.Professional = professional;
		this.Case = @case;
	}
}
