using Godot;
using System;

// ############################################################
//
// ############################################################
[GlobalClass]
public partial class BaseModifierData : Resource
{
   [Export] public Texture2D m_Sprite;
   [Export] public string m_BaseDescription;

    public BaseModifier CreateModifierFromData(ModifierDisplay display) 
    {
        BaseModifier newModifier = _CreateModifier(); 
        newModifier.Init(this, display);
        return newModifier;
    }
   
    protected virtual BaseModifier _CreateModifier() { return new BaseModifier();  }
}

// ############################################################
//
// ############################################################
public class BaseModifier
{
    protected BaseModifierData m_data;
    protected ModifierDisplay m_Display;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Init(BaseModifierData data, ModifierDisplay display) 
    { 
        m_data = data; 
        m_Display = display;
        m_Display.UpdateSprite(m_data.m_Sprite);
        _Init();
        _UpdateDisplay();
        UpdateModifierDescription();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Clear()
    {
        m_Display.QueueFree();
        _Clear();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected virtual void _Init()
    {
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected virtual void _Clear()
    {
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected virtual void _UpdateDisplay()
    {
        m_Display.UpdateLabel(false, new Color(), 0);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    protected virtual void UpdateModifierDescription()
    {
        m_Display.UpdateDescription(m_data.m_BaseDescription);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateModifierDisplayPosition(Vector2 position)
    {
        m_Display.Position = position;
    }
}
