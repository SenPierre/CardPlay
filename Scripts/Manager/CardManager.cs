using Godot;
using System;
using System.Collections.Generic;

public enum CardZone
{
    Deck,
    Hand,
    Discard,
}

public partial class CardManager : Node2D
{
    // STATIC PART ----------------------------
    static CardManager g_Manager;

    static public CardManager GetManager()
    {
        return g_Manager;
    } 
    // STATIC PART ----------------------------
    [Export] public CardDeckData m_StarterDeck;

    public List<CardData> m_RunDeck = new List<CardData>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Ready()
    {        
        g_Manager = this;
        base._Ready();
        GenerateDeck();
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
    public void GenerateDeck()
    {
        m_RunDeck.Clear();

        foreach(CardData data in m_StarterDeck.m_Cards)
        {
            m_RunDeck.Add(data);
        }
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void AddCardDataToDeck(CardData data)
    {
        m_RunDeck.Add(data);
    }
}
