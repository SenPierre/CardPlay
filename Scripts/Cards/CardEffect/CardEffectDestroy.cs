using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectDestroy : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        foreach(Vector2I coord in elementList)
        {
            Element el = gameBoard.GetElement(coord);
            el.m_ToDelete = true;
        }
        gameBoard.SetStateToDestroyElement();
    }
}