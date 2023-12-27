using Godot;
using System;
using System.Collections.Generic;

public partial class MapNode : Node2D
{
    [Export] public Sprite2D m_Sprite;
    [Export] public Button m_Button;
    [Export] public AnimationPlayer m_AnimPlayer;

    public bool m_isEnabled = false;
    public MapNodeData m_NodeDatas;
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Init(MapNodeData data)
    {
        m_NodeDatas = data;
        Position = data.m_Position;
        m_Sprite.Texture = MapNodeData.GetTypeSprite(data.m_Type);
        UpdateAnim();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void SetEnabled(bool enabled)
    {
        m_isEnabled = enabled;
        UpdateAnim();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateAnim()
    {
        if (m_isEnabled)
        {
            m_AnimPlayer.Play("Yes");
        }
        else
        {
            m_AnimPlayer.Play("No");
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnNodeSelection()
    {
        if (m_isEnabled)
        {
            GameManager.GetManager().MapNodeSelected(this);
        }
    }
}
