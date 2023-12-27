using Godot;
using System;
using System.Collections.Generic;

public partial class MapManager : Node2D
{
    // STATIC PART ----------------------------
    static MapManager g_Manager;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    static public MapManager GetManager()
    {
        return g_Manager;
    } 

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static MapData GenerateMap(bool hasGrandma, int deepness, int minWidth, int maxWidth)
    {
        MapData datas = new MapData();
        List<MapNodeData> previousDeepnessNodes;
        List<MapNodeData> currentDeepnessNodes = new List<MapNodeData>();

        Vector2 deepPosition = Vector2.Zero;

        // Setup the firsts Node
        MapNodeData startNode = datas.AddNewNode(MapNodeDataType.Start, deepPosition);
        currentDeepnessNodes.Add(startNode);

        // Is there grandma in this level ?
        if (hasGrandma)
        {
            deepPosition += Vector2.Right * 100.0f;
            currentDeepnessNodes.Clear();
            startNode.m_NextNodeIndexs.Add(1);
            MapNodeData GrandmaNode = datas.AddNewNode(MapNodeDataType.Grandma, deepPosition);
            currentDeepnessNodes.Add(GrandmaNode);
        }

        // Our level will be a certain amount of encounter.
        for (int iDeep = 0; iDeep < deepness; iDeep++)
        {
            deepPosition += Vector2.Right * 100.0f;
            previousDeepnessNodes = currentDeepnessNodes;
            currentDeepnessNodes = new List<MapNodeData>();
            int numNodeInThisDeepness = RandomManager.GetIntRange(minWidth, maxWidth);
            // Create spot for 
            for (int iNode = 0; iNode < numNodeInThisDeepness; iNode++)
            {
                Vector2 spotPosition = deepPosition + Vector2.Down * 100.0f * ((float)iNode - ((float)numNodeInThisDeepness - 1.0f) / 2.0f);
                MapNodeData newNode = datas.AddNewNode(MapNodeData.GetRandomType(), spotPosition);
                currentDeepnessNodes.Add(newNode);
            }
            
            int iNodeN = currentDeepnessNodes.Count -1;
            int iNodeNMinusOne = previousDeepnessNodes.Count -1;
            // Create links of spots
            while (iNodeN > 0 || iNodeNMinusOne > 0)
            {
                previousDeepnessNodes[iNodeNMinusOne].AddNextNode(currentDeepnessNodes[iNodeN].m_Index);
            
                
                if (iNodeN == 0)
                {
                    iNodeNMinusOne--;
                }
                else if (iNodeNMinusOne == 0 || RandomManager.CoinToss())
                {
                    iNodeN--;
                }
                else
                {
                    iNodeNMinusOne--;
                    if (RandomManager.CoinToss())
                    {
                        iNodeN--;
                    }
                }
            }
            previousDeepnessNodes[0].AddNextNode(currentDeepnessNodes[0].m_Index);
        }

        return datas;
    }
    // STATIC PART ----------------------------

    [Export] public Node2D m_MapStartNode;
    [Export] public PackedScene m_NodePrefab;
    [Export] public PackedScene m_LinkPrefab;

    public int m_CurrentNodeIndex = 0;
    public List<MapNode> m_AllNode = new List<MapNode>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void SetCurrentNodeIndex(int index)
    {
        m_CurrentNodeIndex = index;
        UpdateNodeIndexSelection();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Process(double delta)
    {
  
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Init(MapData datas)
    {
        g_Manager = this;
        foreach(MapNodeData data in datas.m_AllMapNodeData)
        {
            MapNode newNode = m_NodePrefab.Instantiate<MapNode>();
            m_MapStartNode.AddChild(newNode);
            m_AllNode.Add(newNode);
            newNode.Init(data);
        }
        
        foreach(MapNode node in m_AllNode)
        {
            foreach(int targetIndex in node.m_NodeDatas.m_NextNodeIndexs)
            {
                MapNode targetNode = m_AllNode[targetIndex];
                AddNodeLink(node, targetNode);
            }
        }

        UpdateNodeIndexSelection();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateNodeIndexSelection()
    {
        if (m_AllNode.Count > 0)
        {
            MapNode currentNode = m_AllNode[m_CurrentNodeIndex];
            foreach(MapNode node in m_AllNode)
            {
                node.SetEnabled(currentNode.m_NodeDatas.m_NextNodeIndexs.Contains(node.m_NodeDatas.m_Index));
            }
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void AddNodeLink(MapNode nodeA, MapNode nodeB)
    {
        Line2D newLink = m_LinkPrefab.Instantiate<Line2D>();
        m_MapStartNode.AddChild(newLink);
        newLink.SetPointPosition(0, nodeA.Position.MoveToward(nodeB.Position, 30.0f));
        newLink.SetPointPosition(1, nodeB.Position.MoveToward(nodeA.Position, 30.0f));
        
    }
}
