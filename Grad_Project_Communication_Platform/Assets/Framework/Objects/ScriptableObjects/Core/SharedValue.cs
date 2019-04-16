using UnityEngine;

namespace Framework.ScriptableObjects.Variables
{
	public class SharedValue<T> : ScriptableObject
	{
		[SerializeField] protected T value;

		public virtual T Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
	}
}
