using System.Collections.Generic;
using Godot;

public enum QueueFuncCall
{
	Activation,
	Update
}

public delegate bool QueueFunc(QueueFuncCall call);

public class StateQueue
{
	private List<QueueFunc> m_Queue = new List<QueueFunc>();
	private QueueFunc m_CurrentQueueFunction;

	public void AddToStartOfTheQueue(QueueFunc a_newFunc)
	{
		m_Queue.Insert(0,a_newFunc);
	}

	public void AddToTheQueue(QueueFunc a_newFunc)
	{
		m_Queue.Add(a_newFunc);
	}

	public void UpdateQueue()
	{
		if (m_CurrentQueueFunction == null)
		{
			_Dequeue();
		}

		if (m_CurrentQueueFunction(QueueFuncCall.Update))
		{
			_Dequeue();
		}
	}

	private void _Dequeue()
	{
		do {
			Debug.Assert(m_Queue.Count > 0, "No more function in the queue");
			m_CurrentQueueFunction = m_Queue[0];
			m_Queue.RemoveAt(0);
		} while (m_CurrentQueueFunction(QueueFuncCall.Activation));
			
	}

	public bool IsCurrentState(QueueFunc a_State)
	{
		return m_CurrentQueueFunction == a_State;
	}

#if TOOLS
	public void DebugDraw(Node2D node)
	{
		Font font = ResourceLoader.Load<Font>("res://debugFont.tres");
		Vector2 offset = new Vector2(30.0f, 30.0f);
		if (m_CurrentQueueFunction != null)
		{
			node.DrawString(font, offset, m_CurrentQueueFunction.Method.ToString(), HorizontalAlignment.Left, -1, 16, new Color(0.0f, 1.0f, 0.0f));

			foreach(QueueFunc func in m_Queue)
			{
				offset += new Vector2(0.0f, 20.0f);
				node.DrawString(font, offset, func.Method.ToString());
			}
		}
		else
		{
			node.DrawString(font, offset, "No function set !", HorizontalAlignment.Left, -1, 16, new Color(1.0f, 0.0f, 0.0f));

		}
	} 
#endif
}
