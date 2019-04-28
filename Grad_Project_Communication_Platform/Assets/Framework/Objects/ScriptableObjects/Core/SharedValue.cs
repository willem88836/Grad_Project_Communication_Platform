using System;
using UnityEngine;

namespace Framework.ScriptableObjects.Variables
{
	public class SharedValue<T> : ScriptableObject
	{
		public Type ValueType { get { return typeof(T); } }

		[SerializeField] protected T value;

		public virtual T Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
	}
}
