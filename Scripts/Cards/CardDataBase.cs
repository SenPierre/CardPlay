using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class CardDataBase
{
    List<CardData> m_AllCardData = new List<CardData>();

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Init()
    {
        string[] allCardsData = Directory.GetFiles("Resources/Cards", "*", SearchOption.AllDirectories);
        foreach(string cardDataFile in allCardsData)
        {
            CardData cardData = ResourceLoader.Load<CardData>(cardDataFile);
            if (cardData.m_Collectible)
            {
                m_AllCardData.Add(cardData);
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public CardData GetRandomCard()
    {
        return m_AllCardData[RandomManager.GetIntRange(0, m_AllCardData.Count - 1)];
    }
}
