using Godot;
using System;
using System.Collections.Generic;

public partial class Card : Node2D
{
    [Export] public Label m_ManaLabel;
    [Export] public Label m_DescLabel;
    [Export] public Sprite2D m_CardImage;

    public int m_ManaCost;
    public bool m_MarkedForExhaust = false;
    BaseCardSelection m_Selection;
    BaseCardEffect[] m_Effects;

    bool m_SelectionPressedLastFrame = false;
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static Card CreateCardFromCardData(CardData data)
    {
        Card newCard = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Cards/Card.tscn").Instantiate<Card>();
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
        InputEventMouse mouseEvent = (InputEventMouse)generatedEvent;
        int buttonLeftPressed = ((int)mouseEvent.ButtonMask) & ((int)MouseButton.Left);
        if (buttonLeftPressed != 0)
        {
            GameManager.GetManager().SelectingACard(this);
            m_Selection.Reset();
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateCardSelection(ElementBoard board, InputEventMouse mouseEvent)
    {
        
        Vector2 pos = mouseEvent.Position - board.Position;

        Vector2I index = new Vector2I((int)pos.X, (int)pos.Y) / (int)Element.ElementSize;
        Vector2 intraElementSelectPos = (pos / Element.ElementSize) - index - new Vector2(0.5f, 0.5f);

        int buttonLeftPressed = ((int)mouseEvent.ButtonMask) & ((int)MouseButton.Left);     

        if (buttonLeftPressed == 0 && m_SelectionPressedLastFrame == false)
        {
            UpdatePreview(board, index, intraElementSelectPos);
        }
        else
        {
            ApplyCardToBoard(board, index, intraElementSelectPos, mouseEvent);
        }

        m_SelectionPressedLastFrame = buttonLeftPressed != 0;  
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ApplyCardToBoard(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset, InputEventMouse mouseEvent)
    {
        m_Selection.Select(gameBoard, selectedElement, clickCenterOffset, mouseEvent);
        if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionInvalid)
        {
            BattleManager.GetManager().DropCard();
        }
        else if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionComplete)
        {
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

    //
}
