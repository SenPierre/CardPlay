using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EnemyIntentMoveVoidAround : BaseEnemyIntent
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        List<Vector2I> voidCoordinates = new List<Vector2I>();
        List<Vector2I> otherCoordinates = new List<Vector2I>();
        ElementBoard board = ElementBoard.GetBoard();
        for (int x = 0; x < board.m_Size; x++)
        {
            for (int y = 0; y < board.m_Size; y++)
            {
                if (board.GetElement(x, y).m_Type == ElementType.Void)
                {
                    voidCoordinates.Add(new Vector2I(x, y));
                }
                else
                {
                    otherCoordinates.Add(new Vector2I(x, y));
                }
            }
        }

        RandomManager.RandomizeList(ref otherCoordinates);

        if (voidCoordinates.Count < otherCoordinates.Count)
        {
            otherCoordinates.RemoveRange(voidCoordinates.Count, otherCoordinates.Count - voidCoordinates.Count);
        }
        else if (voidCoordinates.Count > otherCoordinates.Count)
        {
            voidCoordinates.RemoveRange(otherCoordinates.Count, voidCoordinates.Count - otherCoordinates.Count);
        }
        // Swap take an array and swap the first with the last, the second with the second last, etc...
        // So we fusion both array and the swap will happen naturally.
        voidCoordinates.AddRange(otherCoordinates);
        board.Swap(voidCoordinates);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
    }
}
