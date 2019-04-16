using Framework.Variables;
using UnityEngine;

namespace Framework.Utils
{
	public static class Int2Utilities
	{
		public static Vector2 ToVector2(this Int2 int2)
		{
			return new Vector2(int2.X, int2.Y);
		}

		public static Vector3 ToVector3Up(this Int2 int2)
		{
			return new Vector3(int2.X, int2.Y, 0);
		}

		public static Vector3 ToVector3Forward(this Int2 int2)
		{
			return new Vector3(int2.X, 0, int2.Y);
		}
	}
}
