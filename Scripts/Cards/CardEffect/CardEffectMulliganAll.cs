using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardEffectMulliganAll : BaseCardEffect
{
    public override void ApplyEffect(ElementBoard gameBoard, Card card, List<Vector2I> elementList)
    {
        int cardToDraw = BattleManager.GetManager().GetHandCount() - 1;

        BattleManager.GetManager().DiscardAll(true);

        BattleManager.GetManager().DrawCard(cardToDraw);
    }
}