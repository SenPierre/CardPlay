using Godot;
using System;

// ############################################################
//
// ############################################################
[GlobalClass]
public partial class ModifierTurnBasedData : BaseModifierData
{
   [Export] public int m_TurnDuration;

    protected override BaseModifier _CreateModifier() { return new ModifierChangeMatchCount();}
}

// ############################################################
//
// ############################################################
public class ModifierTurnBased : BaseModifier
{
    protected int m_remainingTurn;

    protected override void _Init()
    {
        base._Init();
        m_remainingTurn = ((ModifierTurnBasedData)m_data).m_TurnDuration;
        BattleManager.GetManager().OnTurnEnd += _OnTurnEnd;
    }

    protected override void _Clear()
    {
        BattleManager.GetManager().OnTurnEnd -= _OnTurnEnd;
        base._Clear();
    }

    protected virtual void _OnTurnEnd()
    {
        if (((ModifierTurnBasedData)m_data).m_TurnDuration > 0)
        {
            m_remainingTurn--;
            if (m_remainingTurn <= 0)
            {
                BattleManager.GetManager().RemoveModifier(this);
            }
            _UpdateDisplay();
            UpdateModifierDescription();
        }
    }

    protected override void _UpdateDisplay()
    {
        m_Display.UpdateLabel(true, new Color(), m_remainingTurn);
    }
}
