using Godot;
using System;

// ############################################################
//
// ############################################################
[GlobalClass]
public partial class ModifierBlockMatchUntilEndOfTurnData : ModifierTurnBasedData
{
    protected override BaseModifier _CreateModifier() { return new ModifierBlockMatchUntilEndOfTurn();}
}

// ############################################################
//
// ############################################################
public class ModifierBlockMatchUntilEndOfTurn : ModifierTurnBased
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Init()
    {
        base._Init();
        ElementBoard.GetBoard().RequestBlockingCheckUntilEndOfTurn();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Clear()
    {
        ElementBoard.GetBoard().RequestUnblockingCheckUntilEndOfTurn();
        base._Clear();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _UpdateDisplay()
    {
        m_Display.UpdateLabel(true, new Color(1.0f, 1.0f, 1.0f) , m_remainingTurn);
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void UpdateModifierDescription()
    {
        string desc = m_data.m_BaseDescription;
        desc = desc.Replace("[X]", m_remainingTurn.ToString());
        m_Display.UpdateDescription(desc);
    }
}
