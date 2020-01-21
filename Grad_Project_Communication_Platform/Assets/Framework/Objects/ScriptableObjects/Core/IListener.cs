namespace Framework.ScriptableObjects.Events
{
	public interface IListener<T>
	{
		void Raise(T value);
	}
}
