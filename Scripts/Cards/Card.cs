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
    BaseCardEffect m_Effect;
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static Card CreateCardFromCardData(CardData data)
    {
        Card newCard = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Cards/Card.tscn").Instantiate<Card>();
        newCard.m_ManaCost = data.m_ManaCost;
        newCard.m_Selection = data.m_Selection;
        newCard.m_Effect =  data.m_Effect;

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
    public void ApplyCardToBoard(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset)
    {
        m_Selection.Select(gameBoard, selectedElement, clickCenterOffset);
        if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionInvalid)
        {
            BattleManager.GetManager().DropCard();
        }
        else if (m_Selection.m_SelectionStatus == SelectionStatus.SelectionComplete)
        {
            List<Vector2I> list = m_Selection.m_ElementList;
            m_Effect.ApplyEffect(gameBoard, this, list);
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
    public Card Clone()
    {
        Card newCard = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Cards/Card.tscn").Instantiate<Card>();
        newCard.m_ManaCost = m_ManaCost;
        newCard.m_Selection = m_Selection;
        newCard.m_Effect =  m_Effect;

        newCard.m_ManaLabel.Text = m_ManaCost.ToString();
        newCard.m_DescLabel.Text = m_DescLabel.Text;

        newCard.m_MarkedForExhaust = false;

        return newCard;
    }
}
