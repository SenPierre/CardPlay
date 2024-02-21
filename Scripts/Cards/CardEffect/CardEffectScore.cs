using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectScore : BaseCardEffect
{
    [Export] public int m_Score = 1000;
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        BattleManager.GetManager().AddToScore(m_Score);
    }
}