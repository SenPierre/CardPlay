using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionElementChunk : BaseCardSelection
{
    [Export] int m_MinChunkSize = 0;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, InputEventMouse mouseEvent)
    {
        gameBoard.ComputeChunkElement(ref m_ElementList, selectedElement);
        
        if (m_ElementList.Count >= m_MinChunkSize)
        {
            m_SelectionStatus = SelectionStatus.SelectionComplete;
        }
        else
        {
            m_SelectionStatus = SelectionStatus.SelectionInvalid;
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        List<Vector2I> selection = new List<Vector2I>();
        gameBoard.ComputeChunkElement(ref selection, selectedElement);

        if (selection.Count >= m_MinChunkSize)
        {
            foreach (Vector2I coord in selection)
            {
                gameBoard.m_Helper.AddHint(coord, coord, BattleManager.GetManager().m_HintColor);
            }
        }
    }
}