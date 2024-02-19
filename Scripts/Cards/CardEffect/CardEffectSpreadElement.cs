using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectSpreadElement : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        Vector2I[] sides = { Vector2I.Up, Vector2I.Left, Vector2I.Down, Vector2I.Right };

        foreach(Vector2I coord in elementList)
        {
            Element el = gameBoard.GetElement(coord);
            foreach (Vector2I side in sides)
            {
                Vector2I coordSide = coord + side;
                if (gameBoard.CheckCoordinateForTransform(coordSide))
                {
                    Element sideEl = gameBoard.GetElement(coordSide);
                    sideEl.TransformElement(el.m_Type);
                }
            }
        }

        gameBoard.RequestToMoveElement();
    }
}