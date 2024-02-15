using Godot;

[GlobalClass]
public partial class TreasureData : Resource
{
    [Export] public Texture2D m_Sprite;
    [Export] public BaseTreasureEffect[] m_Effects = new BaseTreasureEffect[1];
    [Export] public string m_Name;
    [Export] public string m_Description;
    [Export] public bool m_Collectible;
}