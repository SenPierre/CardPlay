using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectDamagePlayer : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        BattleManager.GetManager().DamagePlayer();
    }
}