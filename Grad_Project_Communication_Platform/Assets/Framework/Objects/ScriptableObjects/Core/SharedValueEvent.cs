using Framework.ScriptableObjects.Variables;
using System.Collections.Generic;

namespace Framework.ScriptableObjects.Events
{
	public class SharedValueEvent<T> : SharedValue<T>
	{
		protected List<IListener<T>> listeners = new List<IListener<T>>();


		public override T Value
		{
			get { return this.value; }
			set
			{
				this.value = value;
				Raise();
			}
		}


		public void AddListener(IListener<T> listener)
		{
			if (!listeners.Contains(listener))
				listeners.Add(listener);
		}

		public void RemoveListener(IListener<T> listener)
		{
			if (listeners.Contains(listener))
				listeners.Remove(listener);
		}

		public void Clear()
		{
			listeners.Clear();
		}


		protected void Raise()
		{
			foreach(IListener<T> listener in listeners)
				listener.Raise(this.value);
		}
	}
}
