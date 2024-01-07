using Godot;
using System;

// ############################################################
//
// ############################################################
[GlobalClass]
public partial class ModifierChangeMatchCountData : ModifierTurnBasedData
{
   [Export] public int m_MatchModifier = 1;

    protected override BaseModifier _CreateModifier() { return new ModifierChangeMatchCount();}
}

// ############################################################
//
// ############################################################
public class ModifierChangeMatchCount : ModifierTurnBased
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Init()
    {
        base._Init();
        ElementBoard.GetBoard().m_MatchCount += ((ModifierChangeMatchCountData)m_data).m_MatchModifier;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Clear()
    {
        ElementBoard.GetBoard().m_MatchCount -= ((ModifierChangeMatchCountData)m_data).m_MatchModifier;
        base._Clear();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _UpdateDisplay()
    {
        bool good = ((ModifierChangeMatchCountData)m_data).m_MatchModifier < 0;
        m_Display.UpdateLabel(true, good ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 0.0f, 0.0f), m_remainingTurn);
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void UpdateModifierDescription()
    {
        int matchMod = ((ModifierChangeMatchCountData)m_data).m_MatchModifier;
        string desc = m_data.m_BaseDescription;
        desc = desc.Replace("[X]", m_remainingTurn.ToString());
        desc = desc.Replace("[Y]", (matchMod >= 0 ? "+" : "") + matchMod.ToString() );
        m_Display.UpdateDescription(desc);
    }
}
