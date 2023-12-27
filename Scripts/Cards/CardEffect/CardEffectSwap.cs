using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectSwap : BaseCardEffect
{
    int testUse = 0;
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        gameBoard.Swap(elementList);
        gameBoard.SetStateToMoveElement();
    }
}