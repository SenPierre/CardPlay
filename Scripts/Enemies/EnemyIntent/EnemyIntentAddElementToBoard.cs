using Godot;
using System;
using System.Collections.Generic;


[GlobalClass]
public partial class EnemyIntentAddElementToBoard : BaseEnemyIntent
{
    // TODO
    [Export] public int m_Count;
    [Export] public ElementType m_Type;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        ElementBoard gameBoard = ElementBoard.GetBoard();
        for (int i = 0; i < m_Count; i++)
        {
            Vector2I newRandom = new Vector2I();

            do {
                newRandom.X = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
                newRandom.Y = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
            } while (gameBoard.CheckCoordinate(newRandom) == false);

            
            Element el = gameBoard.GetElement(newRandom);
            el.TransformElement(m_Type);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
        enemy.m_DebuffSprite.Visible = true;
        enemy.m_DebuffSprite.Position = m_Offset;
    }
}
