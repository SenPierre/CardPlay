using System.Collections.Generic;
using Godot;

public partial class ElementBehaviorMold : BaseElementBehavior
{

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void InitBehavior(Element el) 
    {
        base.InitBehavior(el);
        EventManager.GetManager().OnTurnEnd += OnTurnEnd;

    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ClearBehavior() 
    {
        EventManager.GetManager().OnTurnEnd -= OnTurnEnd;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnTurnEnd()
    {
        ElementBoard board = ElementBoard.GetBoard();
        List<Vector2I> directionList = new List<Vector2I>();
        directionList.Add(Vector2I.Down);
        directionList.Add(Vector2I.Left);
        directionList.Add(Vector2I.Up);
        directionList.Add(Vector2I.Right);
        RandomManager.RandomizeList(ref directionList);
        Vector2I currentMoldPos = board.GetElementCoordinate(m_element);

        while (directionList.Count > 0)
        {
            Vector2I chosenNewMold = directionList[0] + currentMoldPos;
            directionList.RemoveAt(0);
            if (board.CheckCoordinateForTransform(chosenNewMold))
            {
                Element el = board.GetElement(chosenNewMold);
                if (el.CanBeDestroyed() 
                && el.m_Type != ElementType.Mold 
                && board.CheckIfElementCreateMatchAtCoordinate(chosenNewMold, ElementType.Mold) == false)
                {
                    el.TransformElement(ElementType.Mold);
                    return;
                }
            }
        }
        
        board.RequestToMoveElement();
    }
}