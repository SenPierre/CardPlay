using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


[GlobalClass]
public partial class EnemyIntentCreateRockFortress : BaseEnemyIntent
{
    // TODO
    [Export] public int m_MinChunkSize = 1;
    [Export] public int m_MaxChunkSize = 4;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        Vector2I randomPos;
        List<Vector2I> chunk = new List<Vector2I>();
        ElementBoard board = ElementBoard.GetBoard();

        do {
            randomPos = new Vector2I(RandomManager.GetIntRange(0, board.m_Size), RandomManager.GetIntRange(0, board.m_Size));
            if (board.CheckCoordinate(randomPos) && board.GetElement(randomPos).m_Type != ElementType.RockElement)
            {
                board.ComputeChunkElement(ref chunk, randomPos);
            }
            
        } while (chunk.Count < m_MinChunkSize || chunk.Count > m_MaxChunkSize);

        ElementType type = board.GetElement(randomPos).m_Type;
        foreach (Vector2I coord in chunk)
        {
            _CheckTransform(coord + Vector2I.Left, type);
            _CheckTransform(coord + Vector2I.Up, type);
            _CheckTransform(coord + Vector2I.Right, type);
            _CheckTransform(coord + Vector2I.Down, type);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _CheckTransform(Vector2I coordinate, ElementType type)
    {
        ElementBoard board = ElementBoard.GetBoard();
        if (board.CheckCoordinateForTransform(coordinate))
        {
            Element el = board.GetElement(coordinate);
            if (el != null && el.m_Type != type)
            {
                el.TransformElement(ElementType.RockElement);
            }
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
    }
}
