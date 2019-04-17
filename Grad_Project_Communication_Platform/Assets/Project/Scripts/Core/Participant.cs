using System;

[Serializable]
public class Participant
{
	public string Name;
	public string IP;
	public string Id;

	public Participant(string name, string iP, string id)
	{
		this.Name = name;
		this.IP = iP;
		this.Id = id;
	}
}
