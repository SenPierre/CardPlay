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
        _ComputeChunkElement(gameBoard, ref m_ElementList, selectedElement);
        m_SelectionStatus = SelectionStatus.SelectionComplete;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        List<Vector2I> selection = new List<Vector2I>();
        _ComputeChunkElement(gameBoard, ref selection, selectedElement);
        foreach (Vector2I coord in selection)
        {
            gameBoard.m_Helper.AddHint(coord, coord, BattleManager.GetManager().m_HintColor);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _ComputeChunkElement(ElementBoard gameBoard, ref List<Vector2I> listToCompute, Vector2I selectedElement)
    {
        Element element = gameBoard.GetElement(selectedElement);
        _ComputeChunkElementRec(gameBoard, ref listToCompute, selectedElement, element.m_Type);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _ComputeChunkElementRec(ElementBoard gameBoard, ref List<Vector2I> listToCompute, Vector2I selectedElement, ElementType elementType)
    {
        Element element = gameBoard.GetElement(selectedElement);
        if (element == null || element.m_Type != elementType || listToCompute.Contains(selectedElement))
        {
            return;
        }

        listToCompute.Add(selectedElement);
        _ComputeChunkElementRec(gameBoard, ref listToCompute, selectedElement + new Vector2I(0,1), elementType);
        _ComputeChunkElementRec(gameBoard, ref listToCompute, selectedElement + new Vector2I(1,0), elementType);
        _ComputeChunkElementRec(gameBoard, ref listToCompute, selectedElement + new Vector2I(-1,0), elementType);
        _ComputeChunkElementRec(gameBoard, ref listToCompute, selectedElement + new Vector2I(0,-1), elementType);
    }
}