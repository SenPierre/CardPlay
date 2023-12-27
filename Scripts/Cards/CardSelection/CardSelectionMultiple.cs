using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionMultiple : BaseCardSelection
{
    [Export] int m_SelectCount = 2;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        if (gameBoard.CheckCoordinate(selectedElement))
        {
            if (m_ElementList.Contains(selectedElement))
            {
                m_ElementList.Remove(selectedElement);
            }
            else
            {
                m_ElementList.Add(selectedElement);
            }

            m_SelectionStatus = m_ElementList.Count == m_SelectCount ? SelectionStatus.SelectionComplete : SelectionStatus.SelectionIncomplete;
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
        gameBoard.m_Helper.AddHint(selectedElement, selectedElement, BattleManager.GetManager().m_HintColor);
        foreach(Vector2I coord in m_ElementList)
        {
            gameBoard.m_Helper.AddHint(coord, coord, BattleManager.GetManager().m_HintColor);
        }
    }
}