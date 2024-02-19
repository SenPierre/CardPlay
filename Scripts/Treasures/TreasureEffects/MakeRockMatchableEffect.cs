using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MakeRockMatchableEffect : BaseTreasureEffect
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Connect()
    {
        GameManager.GetManager().m_ElementDatabase.GetDataFromType(ElementType.RockElement).m_CanMatch = true;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Disconnect()
    {
        GameManager.GetManager().m_ElementDatabase.GetDataFromType(ElementType.RockElement).m_CanMatch = false;
    }
}