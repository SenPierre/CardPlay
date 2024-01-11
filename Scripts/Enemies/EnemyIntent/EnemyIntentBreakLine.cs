using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EnemyIntentBreakLine : BaseEnemyIntent
{
    [Export] public int m_BreakCount = 3;
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        ElementBoard board = ElementBoard.GetBoard();
        List<Vector2I> list = SearchForAllMiddleOfLine();
        RandomManager.RandomizeList(ref list);
        int countToChange = Math.Min(list.Count, m_BreakCount);
        for (int i = 0; i < countToChange; i++)
        {
            Vector2I coord = list[i];
            Element el = board.GetElement(coord);
            ElementType newType;
            ElementDataBase elementDb = GameManager.GetManager().m_ElementDatabase;
            do
            {
                newType = elementDb.GetRandomElementData();
            } while (newType == el.m_Type || board.CheckIfElementCreateMatchAtCoordinate(coord, newType));
            el.TransformElement(newType);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private List<Vector2I> SearchForAllMiddleOfLine()
    {
        ElementBoard board = ElementBoard.GetBoard();
        List<Vector2I> list = new List<Vector2I>();

        for (int x = 0; x < board.m_Size; x++)
        {
            for (int y = 0; y < board.m_Size; y++)
            {
                Vector2I coord = new Vector2I(x,y);
                if (board.CheckCoordinateForTransform(coord))
                {
                    ElementType typeToCheck = board.GetElement(coord).m_Type;
                    if ((x > 0 && x < board.m_Size - 1 && typeToCheck == board.GetElement(x + 1, y).m_Type && typeToCheck == board.GetElement(x - 1, y).m_Type)
                    ||  (y > 0 && y < board.m_Size - 1 && typeToCheck == board.GetElement(x, y + 1).m_Type && typeToCheck == board.GetElement(x, y - 1).m_Type)
                    )
                    {
                        list.Add(coord);
                    }
                }
            }
        }

        return list;
    }
}
