using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Puzzle : Enemy
{
    public int m_StartingMana = 3;
    
   public CardDeckData m_PuzzleHand;
   public CardDeckData m_PuzzleDeck;
   public CardDeckData m_PuzzleDiscard;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    static public Enemy CreatePuzzleFromData(PuzzleData data)
    {
        Puzzle newPuzzle = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Puzzle.tscn").Instantiate<Puzzle>();

        newPuzzle.m_BattleTarget = data.m_PuzzleTarget;
        newPuzzle.m_BoardLayoutRes = data.m_BoardLayoutRes;

        newPuzzle.m_StartingMana = data.m_StartingMana;
        newPuzzle.m_PuzzleHand = data.m_PuzzleHand;
        newPuzzle.m_PuzzleDeck = data.m_PuzzleDeck;
        newPuzzle.m_PuzzleDiscard = data.m_PuzzleDiscard;

        return newPuzzle;
    }

    public override bool IsAPuzzle() { return true; }
}
