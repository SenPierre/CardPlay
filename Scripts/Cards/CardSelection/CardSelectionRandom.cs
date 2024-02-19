using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionRandom : BaseCardSelection
{
    [Export] int m_RandomCount = 10;
    [Export] public ElementType m_IgnoreElement = ElementType.Void;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, MouseButtonMask mouseButtonMask, Vector2 mousePos)
    {
        if (_CheckCoordinate(gameBoard, selectedElement))
        {
            m_ElementList.Clear();

            for (int i = 0; i < m_RandomCount; i++)
            {
                Vector2I newRandom = new Vector2I();

                do {
                    newRandom.X = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
                    newRandom.Y = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
                } while (_CheckCoordinate(gameBoard, newRandom) == false 
                || m_ElementList.Contains(newRandom) 
                || m_IgnoreElement == gameBoard.GetElement(newRandom).m_Type);
                m_ElementList.Add(newRandom);
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
            Vector2I newRandom = new Vector2I();
            newRandom.X = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
            newRandom.Y = RandomManager.GetIntRange(0,gameBoard.m_Size-1);
            gameBoard.m_Helper.AddHint(newRandom, newRandom, BattleManager.GetManager().m_HintColor);
        }
    }
}