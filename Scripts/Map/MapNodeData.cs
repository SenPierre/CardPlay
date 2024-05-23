using Godot;
using System;
using System.Collections.Generic;

public enum MapNodeDataType
{
    Fight,
    Treasure,
    Puzzle,
    Elite,
    Rest,
    Card,
    Event,
    Start,
    Grandma,
    Boss,
}

public class MapNodeData
{
    // May need  to switch randomization model, it may not work the best
    static public Godot.Collections.Array g_NodeWeightDistribution = new Godot.Collections.Array
    {
        10, // Fight
        1,  // Treasure
        3,  // Puzzle
        0,  // Elite
        1,  // Rest
        10, // Card
        0,  // Event
        0,  // Start
        0,  // Grandma
        0,  // Boss
    };

    public MapNodeDataType m_Type;
    public List<int> m_NextNodeIndexs;
    public Vector2 m_Position;
    public int m_Index;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public static List<MapNodeDataType> GenerateRandomPool(int countMult)
    {
        List<MapNodeDataType> pool = new List<MapNodeDataType>();
        foreach (MapNodeDataType type in Enum.GetValues(typeof(MapNodeDataType)))
        {
            for (int i = 0; i < countMult * (int)g_NodeWeightDistribution[(int)type];i++)
            {
                pool.Add(type);
            }
        }
        
        RandomManager.RandomizeList(ref pool);

        return pool;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public static Texture2D GetTypeSprite(MapNodeDataType type)
    {
        switch (type)
        {
            case MapNodeDataType.Fight: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/BattleIcon.png");
            case MapNodeDataType.Treasure: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/Treasure.png");
            case MapNodeDataType.Puzzle: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/PuzzleIcon.png");
            case MapNodeDataType.Elite: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/EliteIcon.png");
            case MapNodeDataType.Rest: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/BarIcon.png");
            case MapNodeDataType.Card: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/CardIcon.png");
            case MapNodeDataType.Event: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/EventIcon.png");
            case MapNodeDataType.Start: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/StartIcon.png");
            case MapNodeDataType.Grandma: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/GrandmaIcon.png");
            case MapNodeDataType.Boss: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/EliteIcon.png");
        }
        return null;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public MapNodeData(MapNodeDataType type, Vector2 position, int index)
    {
        m_Type = type;
        m_NextNodeIndexs = new List<int>();
        m_Position = position;
        m_Index = index;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void AddNextNode(int index)
    {
        m_NextNodeIndexs.Add(index);
    }
}
