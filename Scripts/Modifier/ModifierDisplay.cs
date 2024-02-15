using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModifierDisplay : Node2D
{
    [Export] public Sprite2D m_Sprite;
    [Export] public Label m_CounterLabel;
    [Export] public Tooltip m_Tooltip;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Ready()
    {
        m_Tooltip.SetVisible(false);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateSprite(Texture2D newSprite)
    {
        m_Sprite.Texture = newSprite;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateLabel(bool visible, Color color, int num)
    {
        m_CounterLabel.Visible = visible;
        m_CounterLabel.SelfModulate = color;
        m_CounterLabel.Text = num.ToString();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateTitle(string text)
    {
        m_Tooltip.SetTitle(text);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateDescription(string text)
    {
        m_Tooltip.SetDescription(text);
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
