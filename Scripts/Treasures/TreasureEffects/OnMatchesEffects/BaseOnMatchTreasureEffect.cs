using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseOnMatchTreasureEffect : BaseTreasureEffect
{
    [Export] public ElementType m_TypeFilter = ElementType.Void;
    [Export] public int m_MatchCountRequired = 4;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Connect()
    {
        EventManager.GetManager().OnIndividualMatch += _OnMatch;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Disconnect()
    {
        EventManager.GetManager().OnIndividualMatch -= _OnMatch;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _OnMatch()
    {
        if ((m_TypeFilter == ElementType.Void || ElementsMatch.s_currentMatch.m_Type == m_TypeFilter)
        &&   m_MatchCountRequired <= ElementsMatch.s_currentMatch.m_Element.Count
        )
        {
            OnMatch();
        }
    }

    public abstract void OnMatch();
}