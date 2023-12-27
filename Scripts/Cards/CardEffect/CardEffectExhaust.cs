using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectExhaust : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        card.m_MarkedForExhaust = true;
    }
}