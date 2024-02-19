using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class LoosePointOnNormalGainPointOnRockDestroyEffect : BaseOnDestroyTreasureEffect
{
    [Export] public int m_PointLostOnNormal = 300;
    [Export] public int m_PointGainedOnRock = 2000;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void OnDestroy()
    {
        if (Element.s_LastDestroyedElement.m_Type >= ElementType.Element1
        && Element.s_LastDestroyedElement.m_Type <= ElementType.Element4)
        {
            BattleManager.GetManager().AddToScore(-m_PointLostOnNormal);
        }
        else if (Element.s_LastDestroyedElement.m_Type == ElementType.RockElement)
        {
            BattleManager.GetManager().AddToScore(m_PointGainedOnRock);
        }
    }
}