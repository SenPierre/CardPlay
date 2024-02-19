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
    Event,
    Start,
    Grandma,
}

public class MapNodeData
{
    // May need  to switch randomization model, it may not work the best
    static public Godot.Collections.Array g_NodeRandomWeight = new Godot.Collections.Array
    {
        150, // Fight
        5,  // Treasure
        10,  // Puzzle
        10,  // Elite
        5,  // Rest
        0,  // Event
        0,  // Start
        0,  // Grandma
    };

    public MapNodeDataType m_Type;
    public List<int> m_NextNodeIndexs;
    public Vector2 m_Position;
    public int m_Index;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public static MapNodeDataType GetRandomType()
    {
        int randomMax = 0;
        foreach (MapNodeDataType type in Enum.GetValues(typeof(MapNodeDataType)))
        {
            randomMax += (int)g_NodeRandomWeight[(int)type];
        }

        int randomGot = RandomManager.GetIntRange(1, randomMax);

        MapNodeDataType newType = MapNodeDataType.Start;
        foreach (MapNodeDataType type in Enum.GetValues(typeof(MapNodeDataType)))
        {
            newType = type;
            randomGot -= (int)g_NodeRandomWeight[(int)type];
            if (randomGot <= 0)
            {
                break;
            }
        }

        return newType;
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
            case MapNodeDataType.Event: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/EventIcon.png");
            case MapNodeDataType.Start: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/StartIcon.png");
            case MapNodeDataType.Grandma: return ResourceLoader.Load<Texture2D>("res://Textures/MapIcon/GrandmaIcon.png");
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
