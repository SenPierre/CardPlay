using Godot;
using System;


[GlobalClass]
public partial class EnemyIntentRotateBoard : BaseEnemyIntent
{
    // TODO
    [Export] public bool m_RandomDir = false;
    [Export] public bool m_Anticlockwise = false;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        bool rotationIsAnticlockwise = m_Anticlockwise;
        if (m_RandomDir)
        {
            rotationIsAnticlockwise = RandomManager.CoinToss();
        }
        ElementBoard board = ElementBoard.GetBoard();
        board.RotateTheBoard(rotationIsAnticlockwise);
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
