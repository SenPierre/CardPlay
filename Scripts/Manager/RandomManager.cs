using Godot;
using System;
using System.Collections.Generic;

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
    static public bool RandomChance(float odds)
    {
        return g_Manager.random.Randf() < odds;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    static public bool CoinToss()
    {
        return RandomChance(0.5f);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    static public void RandomizeList<T>(ref List<T> listToRandomize)
    {
        int n = listToRandomize.Count;  
        while (n > 1) {  
            n--;  
            int k = GetIntRange(0, n - 1);  
            T value = listToRandomize[k];  
            listToRandomize[k] = listToRandomize[n];  
            listToRandomize[n] = value;  
        }  
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
