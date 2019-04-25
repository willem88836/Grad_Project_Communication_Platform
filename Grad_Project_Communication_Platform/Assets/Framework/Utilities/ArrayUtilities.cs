using System;
using System.Collections.Generic;

namespace Framework.Utils
{
	public static class ArrayUtilities
	{
		/// <summary>
		///		Inserts range of variables in the provided array.
		/// </summary>
		public static void Insert<T>(this T[] array, int startIndex, T[] range)
		{
			if (startIndex + range.Length > array.Length)
				throw new IndexOutOfRangeException();

			for(int i = 0; i < range.Length; i++)
			{
				array[startIndex + i] = range[i];
			}
		}
		public static void Insert<T>(this T[] array, ref int startIndex, T[] range)
		{
			Insert(array, startIndex, range);
			startIndex += range.Length;
		}
		public static void Insert<T>(this T[] array, int startIndex, List<T> range)
		{
			if (startIndex + range.Count > array.Length)
				throw new IndexOutOfRangeException();

			for (int i = 0; i < range.Count; i++)
			{
				array[startIndex + i] = range[i];
			}
		}
		public static void Insert<T>(this T[] array, ref int startIndex, List<T> range)
		{
			Insert(array, startIndex, range);
			startIndex += range.Count;
		}

		
		/// <summary>
		///		Returns a subarray from the provided array.
		/// </summary>
		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			T[] subArray = new T[length];
			for (int i = 0; i < length; i++)
			{
				subArray[i] = array[startIndex + i];
			}
			return subArray;
		}
		public static T[] SubArray<T>(this T[] array, ref int startIndex, int length)
		{
			T[] subArray = SubArray(array, startIndex, length);
			startIndex += length;
			return subArray;
		}
		public static List<T> SubList<T>(this List<T> list, int startIndex, int length)
		{
			List<T> subList = new List<T>();
			for (int i = 0; i < length; i++)
			{
				subList.Add(list[startIndex + i]);
			}
			return subList;
		}
		public static List<T> SubList<T>(this List<T> list, ref int startIndex, int length)
		{
			List<T> subList = SubList(list, startIndex, length);
			startIndex += length;
			return subList;
		}
	}
}
