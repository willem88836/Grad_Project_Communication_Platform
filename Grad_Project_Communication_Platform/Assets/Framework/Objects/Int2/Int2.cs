using System;

namespace Framework.Variables
{
	[Serializable]
	public struct Int2
	{
		public int X;
		public int Y;

		public Int2(int x, int y)
		{
			X = x;
			Y = y;
		}


		public static Int2 operator -(Int2 a, Int2 b)
		{
			a.X -= b.X;
			a.Y -= b.Y;
			return a;
		}
		public static Int2 operator +(Int2 a, Int2 b)
		{
			a.X += b.X;
			a.Y += b.Y;
			return a;
		}
		public static Int2 operator *(Int2 a, int b)
		{
			a.X *= b;
			a.Y *= b;
			return a;
		}
		public static Int2 operator *(Int2 a, float b)
		{
			a.X = (int)((float)a.X * b);
			a.Y = (int)((float)a.Y * b);
			return a;
		}
		public static Int2 operator /(Int2 a, int b)
		{
			a.X /= b;
			a.Y /= b;
			return a;
		}
		public static Int2 operator %(Int2 a, int b)
		{
			a.X %= b;
			a.Y %= b;
			return a;
		}
	}
}
