using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MapData : Resource
{
    // Start is always first element
    public List<MapNodeData> m_AllMapNodeData = new List<MapNodeData>();

    public MapNodeData AddNewNode(MapNodeDataType type, Vector2 position)
    {
        MapNodeData newNode = new MapNodeData(type, position, m_AllMapNodeData.Count);
        m_AllMapNodeData.Add(newNode);
        return newNode;
    }
}
