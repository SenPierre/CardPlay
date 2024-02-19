using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectEtheral : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
    }

    public override void OnEndTurn(Card card)
    {
        card.m_MarkedForExhaust = true;
    }
}