using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DrawOnMatchTreasureEffect : BaseOnMatchTreasureEffect
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void OnMatch()
    {
        BattleManager.GetManager().DrawCard(1);
    }
}