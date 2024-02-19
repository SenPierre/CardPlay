using System.Collections.Generic;
using Godot;

public enum QueueFuncCall
{
	Activation,
	Paused,
	Resume,
	Update
}

public delegate bool QueueFunc(QueueFuncCall call);

class QueueElement {
	public int m_priority = 0;
	public QueueFunc m_function = null;

	public QueueElement(QueueFunc func, int priority)
	{
		m_function = func;
		m_priority = priority;
	}
}

public class StateQueue
{
	private List<QueueElement> m_Queue = new List<QueueElement>();
	private QueueElement m_CurrentQueueFunction;
	private bool m_StateWasTakenOver = false;

	public void AddToTheQueue(QueueFunc a_newFunc, int priority)
	{
		int i = 0;
		for (i = 0; i < m_Queue.Count; i++)
		{
			if (m_Queue[i].m_priority < priority)
			{
				break;
			}
		}
		m_Queue.Insert(i, new QueueElement(a_newFunc, priority));
	}

	public void UpdateQueue()
	{
		if (m_CurrentQueueFunction == null)
		{
			_Dequeue();
		}

		m_StateWasTakenOver = false;
		if (m_CurrentQueueFunction.m_function(QueueFuncCall.Update) && m_StateWasTakenOver == false)
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
		} while (m_CurrentQueueFunction.m_function(QueueFuncCall.Activation));
			
	}

	public bool IsCurrentState(QueueFunc a_State)
	{
		return m_CurrentQueueFunction.m_function == a_State;
	}

#if TOOLS
	public void DebugDraw(Node2D node)
	{
		Font font = ResourceLoader.Load<Font>("res://debugFont.tres");
		Vector2 offset = new Vector2(30.0f, 30.0f);
		if (m_CurrentQueueFunction != null)
		{
			node.DrawString(font, offset, m_CurrentQueueFunction.m_function.Method.ToString(), HorizontalAlignment.Left, -1, 16, new Color(0.0f, 1.0f, 0.0f));

			foreach(QueueElement func in m_Queue)
			{
				offset += new Vector2(0.0f, 20.0f);
				node.DrawString(font, offset, func.m_function.Method.ToString());
			}
		}
		else
		{
			node.DrawString(font, offset, "No function set !", HorizontalAlignment.Left, -1, 16, new Color(1.0f, 0.0f, 0.0f));

		}
	} 
#endif
}
