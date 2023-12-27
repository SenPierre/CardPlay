using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionColumn : BaseCardSelection
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        m_ElementList.Clear();

        for (int y = 0; y < gameBoard.m_Size; y++)
        {
            Vector2I elCoord = new Vector2I(selectedElement.X, y);
            if (gameBoard.CheckCoordinate(elCoord))
            {
                m_ElementList.Add(elCoord);
            }
        }
        m_SelectionStatus = SelectionStatus.SelectionComplete;
    }
 
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------   
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        Vector2I min = selectedElement;
        Vector2I max = selectedElement;
        min.Y = 0;
        max.Y = gameBoard.m_Size - 1;
        gameBoard.m_Helper.AddHint(min, max, BattleManager.GetManager().m_HintColor);
    }
}