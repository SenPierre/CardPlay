using Godot;

[GlobalClass]
public partial class CardData : Resource
{
    [Export] public int m_ManaCost;
    [Export] public BaseCardSelection m_Selection;
    [Export] public BaseCardEffect m_Effect;
    [Export] public string m_Name;
    [Export] public string m_Description;
    [Export] public bool m_Collectible;
}