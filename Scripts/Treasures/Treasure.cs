using Godot;
using System;
using System.Collections.Generic;

public partial class Treasure : Node2D
{
    [Export] public Sprite2D m_Sprite;
    [Export] public Tooltip m_Tooltip;
    
    TreasureData m_Data;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static Treasure CreateCardFromCardData(TreasureData data)
    {
        Treasure newTreasure = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Treasure.tscn").Instantiate<Treasure>();
        newTreasure.m_Data = data;

        return newTreasure;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Init()
    {
        m_Sprite.Texture = m_Data.m_Sprite;

        m_Tooltip.SetTitle(m_Data.m_Name);
        m_Tooltip.SetDescription(m_Data.m_Description);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Connect()
    {
        foreach(BaseTreasureEffect effect in m_Data.m_Effects)
        {
            effect.Connect();
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Pickup()
    {
        foreach(BaseTreasureEffect effect in m_Data.m_Effects)
        {
            effect.OnPickup();
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Disconnect()
    {
        foreach(BaseTreasureEffect effect in m_Data.m_Effects)
        {
            effect.Connect();
        }
    }

    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void MouseEnter()
    {
        m_Tooltip.SetVisible(true);
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void MouseExit()
    {
        m_Tooltip.SetVisible(false);
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void BoardInputEvent(Node viewport, InputEvent generatedEvent, int shapeIdx)
    {
        InputEventMouse mouseEvent = (InputEventMouse)generatedEvent;
        m_Tooltip.GlobalPosition = mouseEvent.GlobalPosition;
    }
}
