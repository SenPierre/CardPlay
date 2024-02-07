using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class PuzzleData : Resource
{
   [Export] public int m_PuzzleTarget = 1000;
   [Export] public int m_StartingMana = 3;
   [Export] public string m_BoardLayoutRes;
   [Export] public CardDeckData m_PuzzleHand;
   [Export] public CardDeckData m_PuzzleDeck;
   [Export] public CardDeckData m_PuzzleDiscard;
}
