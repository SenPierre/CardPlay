using Godot;
using System;
using System.Collections.Generic;

public partial class Card : Node2D
{
    [Export] public Label m_ManaLabel;
    [Export] public Label m_DescLabel;
    [Export] public Sprite2D m_CardImage;

    public CardData m_Data;
    public int m_ManaCost;
    public bool m_MarkedForExhaust = false;
    BaseCardSelection m_Selection;
    BaseCardEffect[] m_Effects;

    bool m_SelectionPressedLastFrame = false;
    bool m_Selected = false;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static Card CreateCardFromCardData(CardData data)
    {
        Card newCard = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Cards/Card.tscn").Instantiate<Card>();
        newCard.m_Data = data;
        newCard.m_ManaCost = data.m_ManaCost;
        newCard.m_Selection = data.m_Selection;
        newCard.m_Effects =  data.m_Effects;

        newCard.m_ManaLabel.Text = data.m_ManaCost.ToString();
        newCard.m_DescLabel.Text = data.m_Description;

        newCard.m_MarkedForExhaust = false;

        return newCard;
    }

	// -----------------------------------------------------------------
	// Called when the node enters the scene tree for the first time.
	// -----------------------------------------------------------------
	public override void _Ready()
	{
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void CardInputEvent(Node viewport, InputEvent generatedEvent, int shapeIdx)
    {
        if (m_Selected == false)
        {
            InputEventMouse mouseEvent = (InputEventMouse)generatedEvent;
            int buttonLeftPressed = ((int)mouseEvent.ButtonMask) & ((int)MouseButton.Left);
            if (buttonLeftPressed != 0)
            {
                GameManager.GetManager().SelectingACard(this);
                m_Selection.Reset();
            }
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateCardSelection(ElementBoard board, MouseButtonMask mouseButtonMask, Vector2 mousePos)
    {
        
        Vector2 pos = mousePos - board.Position;

        Vector2I index = new Vector2I((int)pos.X, (int)pos.Y) / (int)Element.ElementSize;
        Vector2 intraElementSelectPos = (pos / Element.ElementSize) - index - new Vector2(0.5f, 0.5f);

        int buttonLeftPressed = ((int)mouseButtonMask) & ((int)MouseButton.Left);     
        int buttonRightPressed = ((int)mouseButtonMask) & ((int)MouseButton.Right);    

        if (buttonRightPressed != 0)
        {
            board.m_Helper.ClearHint();
            BattleManager.GetManager().DropCard();
        }
        else if (buttonLeftPressed == 0 && m_SelectionPressedLastFrame == false)
        {
            m_SelectionPressedLastFrame = buttonLeftPressed != 0;  
            UpdatePreview(board, index, intraElementSelectPos);
        }
        else
        {
            m_SelectionPressedLastFrame = buttonLeftPressed != 0;  
            ApplyCardToBoard(board, index, intraElementSelectPos, mouseButtonMask, mousePos);
        }

    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ApplyCardToBoard(
        ElementBoard gameBoard, 
        Vector2I selectedElement, 
        Vector2 clickCenterOffset, 
        MouseButtonMask mouseButtonMask, 
        Vector2 mousePos)
    {
        m_Selection.Select(gameBoard, selectedElement, clickCenterOffset, mouseButtonMask, mousePos);
        if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionInvalid)
        {
            gameBoard.m_Helper.ClearHint();
            BattleManager.GetManager().DropCard();
            m_SelectionPressedLastFrame = false;
        }
        else if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionComplete)
        {
            m_SelectionPressedLastFrame = false;
            List<Vector2I> list = m_Selection.m_ElementList;

            foreach(BaseCardEffect effect in m_Effects)
            {
                effect.ApplyEffect(gameBoard, this, list);
            }
            m_Selection.m_ElementList.Clear();
            BattleManager.GetManager().DropCard();
            BattleManager.GetManager().PayManaCost(m_ManaCost);
            gameBoard.m_Helper.ClearHint();
            if (m_MarkedForExhaust == false)
            {
                BattleManager.GetManager().Discard(this);
            }
            BattleManager.GetManager().CheckForExhaust();
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdatePreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        gameBoard.m_Helper.ClearHint();
        m_Selection.ApplySelectionPreview(gameBoard, selectedElement, clickCenterOffset);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void GetSelectionAdditionnalData<T>(ref T data)
    {
        m_Selection.GetAdditionnalData(ref data);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnDraw()
    {
        foreach(BaseCardEffect effect in m_Effects)
        {
            effect.OnDraw(this);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnEndTurnDiscard()
    {
        foreach(BaseCardEffect effect in m_Effects)
        {
            effect.OnEndTurn(this);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public Card Clone()
    {
        Card newCard = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Cards/Card.tscn").Instantiate<Card>();
        newCard.m_ManaCost = m_ManaCost;
        newCard.m_Selection = m_Selection;
        newCard.m_Effects =  m_Effects;

        newCard.m_ManaLabel.Text = m_ManaCost.ToString();
        newCard.m_DescLabel.Text = m_DescLabel.Text;

        newCard.m_MarkedForExhaust = false;

        return newCard;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Selected()
    {
        BattleManager.GetManager().AddToStateQueue(Queue_CardSelected, 10);
        m_Selected = true;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Dropped()
    {
        m_Selected = false;
    }


	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public bool Queue_CardSelected(QueueFuncCall a_Call)
    {
		switch (a_Call)
		{
			case QueueFuncCall.Activation: 
			{
				break;
			}
			case QueueFuncCall.Update: 
			{
                ElementBoard board = ElementBoard.GetBoard();                
                UpdateCardSelection(board, Input.GetMouseButtonMask(), GetViewport().GetMousePosition());
                return m_Selected == false;
			}
		}
		// End of battle, never leeeeave
		return false;
    }
}
