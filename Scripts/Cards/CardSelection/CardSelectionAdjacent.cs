using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionAdjacent : BaseCardSelection
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, InputEventMouse mouseEvent)
    {
        m_ElementList.Clear();
        if (_CheckCoordinate(gameBoard, selectedElement) == false)
        {
            m_SelectionStatus = SelectionStatus.SelectionInvalid;
            return;
        }

        Vector2I AdjacentCoordinate = selectedElement + ComputeAdjacentOffset(clickCenterOffset);

        if (_CheckCoordinate(gameBoard, AdjacentCoordinate) == false)
        {
            m_SelectionStatus = SelectionStatus.SelectionInvalid;
            return;
        }

        m_ElementList.Add(selectedElement);
        m_ElementList.Add(AdjacentCoordinate);
        
        m_SelectionStatus = SelectionStatus.SelectionComplete;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        Vector2I AdjacentCoordinate = ComputeAdjacentOffset(clickCenterOffset);
        if (AdjacentCoordinate.X == -1 || AdjacentCoordinate.Y == -1)
        {
            gameBoard.m_Helper.AddHint(selectedElement + AdjacentCoordinate, selectedElement, BattleManager.GetManager().m_HintColor);
        }
        else
        {
            gameBoard.m_Helper.AddHint(selectedElement, selectedElement + AdjacentCoordinate, BattleManager.GetManager().m_HintColor);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private Vector2I ComputeAdjacentOffset(Vector2 clickCenterOffset)
    {
        Vector2I AdjacentOffset;
        if (Mathf.Abs(clickCenterOffset.X) > Mathf.Abs(clickCenterOffset.Y))
        {
            // Horizontal Swap
            if (clickCenterOffset.X > 0.0f)
            {
                AdjacentOffset = new Vector2I(1,0);
            }
            else
            {
                AdjacentOffset = new Vector2I(-1,0);
            }
        }
        else
        {
            // Vertical Swap
            if (clickCenterOffset.Y > 0.0f)
            {
                AdjacentOffset = new Vector2I(0,1);
            }
            else
            {
                AdjacentOffset = new Vector2I(0,-1);
            }
        }
        return AdjacentOffset;
    }

}