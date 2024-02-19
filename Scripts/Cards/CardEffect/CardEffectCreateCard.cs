using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectCreateCard : BaseCardEffect
{
    [Export] public string m_Data;
    [Export] public CardZone m_Zone = CardZone.Deck;
    [Export] public int m_count = 1;

    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        CardData cardData = ResourceLoader.Load<CardData>(m_Data); 
        for(int i = 0; i < m_count; i++)
        {
            Card newCard = Card.CreateCardFromCardData(cardData);
            BattleManager.GetManager().AddCardToZone(newCard, m_Zone);
        }
    }
}