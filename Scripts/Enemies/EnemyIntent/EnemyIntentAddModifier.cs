using Godot;
using System;


[GlobalClass]
public partial class EnemyIntentAddModifier : BaseEnemyIntent
{
    // TODO
    [Export] public BaseModifierData[] m_CardToAdd;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        foreach (BaseModifierData modifier in m_CardToAdd)
        {
            BattleManager.GetManager().AddModifier(modifier);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
    }
}
