using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CardDeckData : Resource
{
    [Export] public CardData[] m_Cards;
}