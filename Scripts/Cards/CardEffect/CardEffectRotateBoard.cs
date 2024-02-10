using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectRotateBoard : BaseCardEffect
{
    [Export] public bool m_Anticlockwise = false;
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        gameBoard.RotateTheBoard(m_Anticlockwise);
        gameBoard.RequestToMoveElement();
    }
}