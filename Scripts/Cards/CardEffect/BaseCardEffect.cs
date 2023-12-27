using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseCardEffect : Resource
{
    public abstract void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList);

}