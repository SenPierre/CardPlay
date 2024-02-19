using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionAll : BaseCardSelection
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
                for (int y = 0; y < gameBoard.m_Size; y++)
                {
                    Vector2I elCoord = new Vector2I(x, y);
                    if (_CheckCoordinate(gameBoard, elCoord))
                    {
                        m_ElementList.Add(elCoord);
                    }
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
            gameBoard.m_Helper.AddHint(new Vector2I(0,0), new Vector2I(gameBoard.m_Size-1,gameBoard.m_Size-1), BattleManager.GetManager().m_HintColor);
        }
    }
}