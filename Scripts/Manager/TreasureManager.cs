using Godot;
using System;
using System.Collections.Generic;

public partial class TreasureManager : Node2D
{
    // STATIC PART ----------------------------
    static TreasureManager g_Manager;

    static public TreasureManager GetManager()
    {
        return g_Manager;
    } 
    // STATIC PART ----------------------------
    [Export] public TreasureData m_TreasureTest;

    public List<Treasure> m_RunTreasures = new List<Treasure>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Ready()
    {        
        g_Manager = this;
        base._Ready();

        GD.Print(m_TreasureTest.m_Effects);
        AddTreasureToRun(m_TreasureTest);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void AddTreasureToRun(TreasureData data)
    {
        Treasure newTreasure = Treasure.CreateCardFromCardData(data);
        AddChild(newTreasure);
        newTreasure.Init();
        newTreasure.Position = Vector2.Right * 100.0f * m_RunTreasures.Count;
        newTreasure.Connect();
        newTreasure.Pickup();
        m_RunTreasures.Add(newTreasure);
    }
}
