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
}


[GlobalClass]
public partial class ElementData : Resource
{
    [Export] public ElementType m_Type;
    [Export] public Texture2D m_Sprite;
    [Export] public int m_RandomWeight;
    [Export] public bool m_CanMatch;
    [Export] public bool m_CanBeDestroyed;
    [Export] public PackedScene m_ElementBehaviorNode;
}