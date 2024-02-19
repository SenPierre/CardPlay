using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseCardEffect : Resource
{
    public virtual void OnDraw(Card card) {}
    public abstract void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList);
    public virtual void OnEndTurn(Card card) {} 
}