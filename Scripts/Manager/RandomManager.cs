using Godot;
using System;

public partial class RandomManager : Node2D
{
    // STATIC PART ----------------------------
    static private RandomManager g_Manager;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    static public int GetIntRange(int min, int max)
    {
        return g_Manager.random.RandiRange(min, max);
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    static public bool CoinToss()
    {
        return g_Manager.random.RandiRange(0, 1) == 1;
    }
    // STATIC PART ----------------------------
    
    private RandomNumberGenerator random = new RandomNumberGenerator();

	// -----------------------------------------------------------------
	// Called when the node enters the scene tree for the first time.
	// -----------------------------------------------------------------
	public override void _Ready()
	{
        g_Manager = this;
        random.Randomize();
	}
}
