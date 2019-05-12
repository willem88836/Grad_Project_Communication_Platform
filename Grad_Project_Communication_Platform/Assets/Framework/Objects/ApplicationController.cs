using UnityEngine;

public abstract class ApplicationController<T> : MonoBehaviour
{
	protected T Manager { get; private set; }

	public virtual void Initialize(T manager)
	{
		this.Manager = manager;
	}
}
