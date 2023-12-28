using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionElementChunk : BaseCardSelection
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        gameBoard.ComputeChunkElement(ref m_ElementList, selectedElement);
        m_SelectionStatus = SelectionStatus.SelectionComplete;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        List<Vector2I> selection = new List<Vector2I>();
        gameBoard.ComputeChunkElement(ref selection, selectedElement);
        foreach (Vector2I coord in selection)
        {
            gameBoard.m_Helper.AddHint(coord, coord, BattleManager.GetManager().m_HintColor);
        }
    }
}