using System.Collections.Generic;
using Godot;

public partial class ElementBehaviorRotator : BaseElementBehavior
{

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void InitBehavior(Element el) 
    {
        base.InitBehavior(el);
        BattleManager.GetManager().OnTurnEnd += OnTurnEnd;

    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ClearBehavior() 
    {
        BattleManager.GetManager().OnTurnEnd -= OnTurnEnd;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnTurnEnd()
    {
        ElementBoard board = ElementBoard.GetBoard();
        
        Vector2I baseCoordinate = board.GetElementCoordinate(m_element);
        
        List<Vector2I> directionList = new List<Vector2I>();
        directionList.Add(baseCoordinate + Vector2I.Down);
        directionList.Add(baseCoordinate + Vector2I.Down + Vector2I.Left);
        directionList.Add(baseCoordinate + Vector2I.Left);
        directionList.Add(baseCoordinate + Vector2I.Up + Vector2I.Left);
        directionList.Add(baseCoordinate + Vector2I.Up);
        directionList.Add(baseCoordinate + Vector2I.Up + Vector2I.Right);
        directionList.Add(baseCoordinate + Vector2I.Right);
        directionList.Add(baseCoordinate + Vector2I.Down + Vector2I.Right);

        for (int i = directionList.Count - 1; i >= 0; i--)
        {
            
            if (board.CheckCoordinateForMovement(directionList[i]))
            {
                board.GetElement(directionList[i]).SetRotationCenter(m_element.GlobalPosition);
            }
            else
            {
                directionList.RemoveAt(i);
            }
        }

        board.OffsetElements(directionList, 1, ElementMovementAnimation.RotateClockwise);
        
        board.SetStateToMoveElement();
    }
}