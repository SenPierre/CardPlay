using Godot;

public enum ElementType {
    Element1,
    Element2,
    Element3,
    Element4,
    // -----------
    RockElement,
    Void,
    // -----------
    Bomb2,
    Mold,
}


[GlobalClass]
public partial class ElementData : Resource
{
    [Export] public ElementType m_Type;
    [Export] public Texture2D m_Sprite;
    [Export] public int m_RandomWeight = 0;
    [Export] public bool m_CanMatch = false;
    [Export] public bool m_CanBeDestroyed = true;
    [Export] public bool m_CanBeMoved = true;
    [Export] public bool m_CanBeTransformed = true;
    [Export] public bool m_CanFall = true;
    [Export] public PackedScene m_ElementBehaviorNode;
}