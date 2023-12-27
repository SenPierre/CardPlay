using Godot;
using System;

[GlobalClass]
public partial class EnemyIntentMult : BaseEnemyIntent
{
    // TODO
    [Export] public BaseEnemyIntent[] m_SubIntent;  

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        foreach(BaseEnemyIntent intent in m_SubIntent)
        {
            intent.ApplyIntent(enemy);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 offset)
    {
        Vector2 newOffset = offset - new Vector2(25.0f, 0.0f) * (m_SubIntent.GetLength(0) - 1);
        foreach(BaseEnemyIntent intent in m_SubIntent)
        {
            intent.ShowIntent(enemy, newOffset);
            newOffset += new Vector2(50.0f, 0.0f);
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override int GetLimitBase() { 
        int limitBase = m_LimitBase;
        foreach(BaseEnemyIntent intent in m_SubIntent)
        {
            limitBase += intent.GetLimitBase();
        }
        return limitBase; 
    }
}
