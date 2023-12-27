using Godot;

public enum StateFunctionCall
{
	Enter,
	Update,
	Exit
};

public delegate StateFunc StateFunc(StateFunctionCall a_Call);

public class StateMachine
{
	private StateFunc m_CurrentStateFunction;
	public bool m_Lock;

	public void SetCurrentStateFunction(StateFunc a_newState)
	{
		Debug.Assert(m_Lock == false, "State Machine Lock disruption. Trying to call SetCurrentStateFunction during state call");
		m_Lock = true;
		if (m_CurrentStateFunction != null)
		{
			m_CurrentStateFunction(StateFunctionCall.Exit);
		}

		m_CurrentStateFunction = a_newState;

		if (m_CurrentStateFunction != null)
		{
			m_CurrentStateFunction(StateFunctionCall.Enter);
		}
		m_Lock = false;
	}

	public void UpdateStateMachine()
	{
		if (m_CurrentStateFunction != null)
		{
			m_Lock = true;
			StateFunc result = m_CurrentStateFunction(StateFunctionCall.Update);
			m_Lock = false;
			if (result != null)
			{
				SetCurrentStateFunction(result);
			}
		}
	}

	public bool IsCurrentState(StateFunc a_State)
	{
		return m_CurrentStateFunction == a_State;
	}
}
