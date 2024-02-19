using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectTransform : BaseCardEffect
{
    [Export] public ElementType newElement; 
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        ElementType trueType = newElement;
        if(trueType == ElementType.BasicElement)
        {
            trueType = GameManager.GetManager().m_ElementDatabase.GetRandomBasicElementType();
        }

        foreach(Vector2I coord in elementList)
        {
            Element el = gameBoard.GetElement(coord);
            el.TransformElement(trueType);
        }
        gameBoard.RequestToMoveElement();
    }
}