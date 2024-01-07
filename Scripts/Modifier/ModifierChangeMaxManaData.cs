using Godot;
using System;

// ############################################################
//
// ############################################################
[GlobalClass]
public partial class ModifierChangeMaxManaData : ModifierTurnBasedData
{
   [Export] public int m_ManaModifier = 1;

    protected override BaseModifier _CreateModifier() { return new ModifierChangeMaxMana();}
}

// ############################################################
//
// ############################################################
public class ModifierChangeMaxMana : ModifierTurnBased
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Init()
    {
        base._Init();
        BattleManager.GetManager().m_MaxMana += ((ModifierChangeMaxManaData)m_data).m_ManaModifier;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _Clear()
    {
        BattleManager.GetManager().m_MaxMana -= ((ModifierChangeMaxManaData)m_data).m_ManaModifier;
        base._Clear();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void _UpdateDisplay()
    {
        bool good = ((ModifierChangeMaxManaData)m_data).m_ManaModifier > 0;
        m_Display.UpdateLabel(true, good ? new Color(0.0f, 1.0f, 0.0f) : new Color(1.0f, 0.0f, 0.0f), m_remainingTurn);
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected override void UpdateModifierDescription()
    {
        int manaMod = ((ModifierChangeMaxManaData)m_data).m_ManaModifier;
        string desc = m_data.m_BaseDescription;
        desc = desc.Replace("[X]", m_remainingTurn.ToString());
        desc = desc.Replace("[Y]", (manaMod >= 0 ? "+" : "") + manaMod.ToString() );
        m_Display.UpdateDescription(desc);
    }
}
