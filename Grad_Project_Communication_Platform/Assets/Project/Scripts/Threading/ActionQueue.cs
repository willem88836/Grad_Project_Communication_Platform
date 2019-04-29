using System;
using System.Collections.Generic;

public class ActionQueue
{
	public static ActionQueue Instance;

	private Queue<Action> actionQueue;


	public ActionQueue()
	{
		actionQueue = new Queue<Action>();
		Instance = this;
	}

	public void Invoke()
	{
		while (actionQueue.Count > 0)
		{
			Action action = actionQueue.Dequeue();
			action.Invoke();
		}
	}


	public static void Enqueue(Action action)
	{
		if (Instance != null)
		{
			Instance.actionQueue.Enqueue(action);
		}
	}
}
