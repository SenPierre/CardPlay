using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionLine : BaseCardSelection
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, MouseButtonMask mouseButtonMask, Vector2 mousePos)
    {
        if (_CheckCoordinate(gameBoard, selectedElement))
        {
            m_ElementList.Clear();

            for (int x = 0; x < gameBoard.m_Size; x++)
            {
                Vector2I elCoord = new Vector2I(x, selectedElement.Y);
                if (_CheckCoordinate(gameBoard, elCoord))
                {
                    m_ElementList.Add(elCoord);
                }
            }
            m_SelectionStatus = SelectionStatus.SelectionComplete;
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        if (_CheckCoordinate(gameBoard, selectedElement))
        {
            Vector2I min = selectedElement;
            Vector2I max = selectedElement;
            min.X = 0;
            max.X = gameBoard.m_Size - 1;
            gameBoard.m_Helper.AddHint(min, max, BattleManager.GetManager().m_HintColor);
        }
    }
}