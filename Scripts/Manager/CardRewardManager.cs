using Godot;
using System;
using System.Collections.Generic;

public partial class CardRewardManager : Node2D
{
    // STATIC PART ----------------------------
    static CardRewardManager g_Manager;

    static public CardRewardManager GetManager()
    {
        return g_Manager;
    } 
    // STATIC PART ----------------------------

    [Export] public Node2D m_ShownCardNode;

    public int m_displayedCardCount = 3;

    List<CardData> m_CardShownData = new List<CardData>();
    List<Card> m_ShownCard = new List<Card>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Ready()
    {        
        g_Manager = this;
        base._Ready();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Process(double delta)
    {
        base._Process(delta);
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Init()
    {
        Vector2 ShownCardNodePosition = m_ShownCardNode.Position;
        ShownCardNodePosition.X = ShownCardNodePosition.X - (125.0f * (m_displayedCardCount - 1));

        for (int i = 0; i < m_displayedCardCount; i++)
        {
            Vector2 offsetShow = new Vector2(250.0f * i, 0.0f);
            CardData newCardData = GameManager.GetManager().m_CardDatabase.GetRandomCard();
            Card newCard = Card.CreateCardFromCardData(newCardData);

            AddChild(newCard);
            newCard.Position = ShownCardNodePosition + offsetShow;
            m_CardShownData.Add(newCardData);
            m_ShownCard.Add(newCard);           
        }
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void SelectCard(Card card)
    {
        for (int i = 0; i < m_displayedCardCount; i++)
        {
            if (card == m_ShownCard[i])
            {
                CardManager.GetManager().AddCardDataToDeck(m_CardShownData[i]);      
                break;          
            }         
        }

        Clear();

        GameManager.GetManager().EndReward();
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Clear()
    {
        for (int i = 0; i < m_displayedCardCount; i++)
        {
            m_ShownCard[i].QueueFree();
        }
        m_ShownCard.Clear();
        m_CardShownData.Clear();
    }
}