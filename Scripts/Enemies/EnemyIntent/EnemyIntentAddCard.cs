using Godot;
using System;


[GlobalClass]
public partial class EnemyIntentAddCard : BaseEnemyIntent
{
    // TODO
    [Export] public CardZone m_TargetZone;
    [Export] public CardData[] m_CardToAdd;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ApplyIntent(Enemy enemy)
    {
        foreach (CardData cardData in m_CardToAdd)
        {
            Card newCard = Card.CreateCardFromCardData(cardData);
            BattleManager.GetManager().AddCardToZone(newCard, m_TargetZone);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ShowIntent(Enemy enemy, Vector2 m_Offset)
    {
        enemy.m_DebuffSprite.Visible = true;
        enemy.m_DebuffSprite.Position = m_Offset;
    }
}
