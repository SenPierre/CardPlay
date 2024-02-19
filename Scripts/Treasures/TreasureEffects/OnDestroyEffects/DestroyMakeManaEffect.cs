using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DestroyMakeManaEffect : BaseOnDestroyTreasureEffect
{
    [Export] public int m_ManaGain;
    [Export] public int m_DestroyCountMax;

    private int m_DestroyCount = 0;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void Connect()
    {
        base.Connect();
        m_DestroyCount = 0;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void OnDestroy()
    {
        m_DestroyCount++;

        if (m_DestroyCount == m_DestroyCountMax)
        {
            m_DestroyCount = 0;
            BattleManager.GetManager().PayManaCost(-1);
        }
    }
}