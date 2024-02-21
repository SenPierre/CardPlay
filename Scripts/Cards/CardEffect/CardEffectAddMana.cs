using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectAddMana : BaseCardEffect
{
    [Export] public int m_Mana = 1;
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        BattleManager.GetManager().PayManaCost(-m_Mana);
    }
}