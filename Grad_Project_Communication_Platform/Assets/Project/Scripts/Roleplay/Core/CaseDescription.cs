using Framework.Features.Json;

public class CaseDescription
{
	// HACK: JsonUtility doesnt work with multidimensional arrays yet, but I don't wanna get into that right now.
	[JsonIgnore] public int[][] Characteristics;
	public string StringifiedCharacteristics
	{
		get
		{
			string a = "";

			for (int i = 0; i < Characteristics.Length;i++)
			{
				int[] b = Characteristics[i];

				for (int j = 0; j < b.Length; j++)
				{
					int c = b[j];

					a += c.ToString();
					if (j < b.Length - 1)
					{
						a += ',';
					}
				}
				if (i < Characteristics.Length - 1)
				{
					a += ';';
				}
			}

			return a;
		}
		set
		{
			string[] splitA = value.Split(';');
			int[][] o = new int[splitA.Length][];

			for (int i = 0; i < splitA.Length; i++)
			{
				string[] splitB = splitA[i].Split(',');

				int[] p = o[i] = new int[splitB.Length];

				for (int j = 0; j < splitB.Length; j++)
				{
					p[j] = int.Parse(splitB[j]);
				}
			}
		}
	}
	public RoleplayModule Module;

	public CaseDescription() { }

	public CaseDescription(int[][] characteristics, RoleplayModule module)
	{
		this.Characteristics = characteristics;
		this.Module = module;
	}
}
