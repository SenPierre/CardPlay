using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardSelectionDragColumn : BaseCardSelection
{
    int m_DragOffset = 2;
	bool m_DragHappening = false;
    float m_DragStartPos = 0.0f;
    int xLineSelected = 0;
    // -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, MouseButtonMask mouseButtonMask, Vector2 mousePos)
    {
        m_ElementList.Clear();

        int buttonLeftPressed = ((int)mouseButtonMask) & ((int)MouseButton.Left);     

        if (buttonLeftPressed == 0 && m_DragHappening) // leaving the drag
        {
            m_DragHappening = false;
            for (int y = 0; y < gameBoard.m_Size; y++)
            {
                Vector2I elCoord = new Vector2I(xLineSelected, y);
                if (_CheckCoordinate(gameBoard, elCoord))
                {
                    m_ElementList.Add(elCoord);
                }
            }
            if (m_DragOffset%m_ElementList.Count == 0)
            {
                m_SelectionStatus = SelectionStatus.SelectionInvalid;
                gameBoard.ResetElementsPosition();
            }
            else
            {
                m_SelectionStatus = SelectionStatus.SelectionComplete;
            }
        }
        else 
        {
            if (m_DragHappening == false)
            {
                if (_CheckCoordinate(gameBoard, selectedElement) == false)
                {
                    return;
                }
                m_DragStartPos = mousePos.Y;
                xLineSelected = selectedElement.X;
                m_DragHappening = true;
            }
            
            float offset = (mousePos.Y - m_DragStartPos) / Element.ElementSize;
            m_DragOffset = (int)Mathf.Round(offset);

            gameBoard.PreviewColumnDrag(xLineSelected, offset);

            m_SelectionStatus = SelectionStatus.SelectionIncomplete;
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        if (_CheckCoordinate(gameBoard, selectedElement) || m_DragHappening)
        {
            Vector2I min = selectedElement;
            Vector2I max = selectedElement;
            min.Y = 0;
            max.Y = gameBoard.m_Size - 1;
            gameBoard.m_Helper.AddHint(min, max, BattleManager.GetManager().m_HintColor);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void GetAdditionnalData<T>(ref T data)
    {
        OffsetData intData = data as OffsetData;
        if (intData != null)
        {
            intData.m_value = m_DragOffset;
        }
    }
}