using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionNone : BaseCardSelection
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, InputEventMouse mouseEvent)
    {        
        m_SelectionStatus = SelectionStatus.SelectionComplete;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        gameBoard.m_Helper.AddHint(new Vector2I(0,0), new Vector2I(gameBoard.m_Size-1,gameBoard.m_Size-1), BattleManager.GetManager().m_HintColor);
    }
}