using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModifierDisplay : Node2D
{
    [Export] public Sprite2D m_Sprite;
    [Export] public Label m_CounterLabel;
    [Export] public Label m_DescriptionLabel;
    [Export] public Node2D m_DescriptionPanel;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Ready()
    {
        m_DescriptionPanel.Visible = false;
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
    public void UpdateDescription(string text)
    {
        m_DescriptionLabel.Text = text;
    }

    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void MouseEnter()
    {
        m_DescriptionPanel.Visible = true;
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void MouseExit()
    {
        m_DescriptionPanel.Visible = false;
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void BoardInputEvent(Node viewport, InputEvent generatedEvent, int shapeIdx)
    {
        InputEventMouse mouseEvent = (InputEventMouse)generatedEvent;
        m_DescriptionPanel.GlobalPosition = mouseEvent.GlobalPosition;
    }
}
