using Godot;
using System;
using System.Collections.Generic;

public class OffsetData
{
    public int m_value = 0;
}

[GlobalClass]
public partial class CardEffectOffset : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        OffsetData offset = new OffsetData();
        card.GetSelectionAdditionnalData(ref offset);

        gameBoard.OffsetElements(elementList, offset.m_value);
        gameBoard.RequestToMoveElement();
    }
}