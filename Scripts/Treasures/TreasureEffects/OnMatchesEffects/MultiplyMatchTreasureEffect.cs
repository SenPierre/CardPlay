using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MultiplyMatchTreasureEffect : BaseOnMatchTreasureEffect
{
    [Export] public float m_Multiplier = 1.0f;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void OnMatch()
    {
        ElementsMatch.s_currentMatch.m_matchScore = (int)Math.Floor(ElementsMatch.s_currentMatch.m_matchScore * m_Multiplier);
    }
}