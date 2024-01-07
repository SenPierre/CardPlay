using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node2D
{
    // STATIC PART ----------------------------
    static BattleManager g_Manager;

    static public BattleManager GetManager()
    {
        return g_Manager;
    } 
    // STATIC PART ----------------------------

    [Export] public Node2D m_EnemyRoot;

    [Export] public TextureProgressBar m_ScoreBar;
    [Export] public TextureRect m_LimitBar;

    [Export] public TextureRect[] m_Hearts;

    [Export] public TextureRect m_GameOverRect;
    
    [Export] public Label m_ManaLabel;
    [Export] public Color m_HintColor;
    [Export] public Line2D m_CardArrow;
    
    [Export] public Node2D m_HandNode;
    [Export] public Node2D m_DeckNode;
    [Export] public Node2D m_DiscardNode;
    
    [Export] public Node2D m_ModifierNode;

    [Export] public int m_MaxMana = 3;

    [Signal]
    public delegate void OnTurnEndEventHandler();

    private int m_enemyIndex = 0;

    private Enemy m_ActiveEnemy = null;
    private int m_CurrentBattleScore = 0;
    private int m_TargetBattleScore = 0;
    private int m_CurrentLifeLimit = 0;

    private int m_PlayerLife = 3;
    
    private StateMachine m_StateMachine = new StateMachine();
    
    List<Card> m_CardDeck = new List<Card>();
    List<Card> m_CardHand = new List<Card>();
    List<Card> m_CardDiscard = new List<Card>();

    public Card m_CurrentHeldCard;

    List<BaseModifier> m_Modifiers = new List<BaseModifier>();

    int m_CurrentMana = 0;
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void InitFight(EnemyData enemyData)
    {
        g_Manager = this;
        
        SetPlayerLife(3);
        m_GameOverRect.Visible = false;
        m_CardArrow.DefaultColor = m_HintColor;
        
        m_ActiveEnemy = Enemy.CreateEnemyFromData(enemyData);
        m_EnemyRoot.AddChild(m_ActiveEnemy);        
        m_ActiveEnemy.UpdateIntentVisual();
        m_TargetBattleScore = m_ActiveEnemy.m_BattleTarget;
        m_CurrentBattleScore = 0;
        ElementBoard.GetBoard().InitBoard(m_ActiveEnemy.m_BoardLayoutRes);
        m_StateMachine.SetCurrentStateFunction(State_StartBattle);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Ready()
    {
        base._Ready();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Process(double delta)
    {
        m_StateMachine.UpdateStateMachine();
        UpdateCardHandPosition();
        UpdateArrow();

        base._Process(delta);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void SetPlayerLife(int val)
    {
        m_PlayerLife = val;
        UpdateUI();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void AddToScore(int val)
    {
        m_CurrentBattleScore += val;

        UpdateUI();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void DamagePlayer()
    {
        m_PlayerLife--;
        UpdateUI();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateLimit()
    {
        m_CurrentLifeLimit = m_ActiveEnemy.GetLimit() + m_CurrentBattleScore;
        m_CurrentLifeLimit = Math.Min(m_CurrentLifeLimit, m_TargetBattleScore);
        UpdateUI();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ApplyEnemyAttack()
    {
        if (m_CurrentLifeLimit > m_CurrentBattleScore)
        {
            DamagePlayer();
        }
        m_ActiveEnemy.ApplyCurrentIntent();
        m_ActiveEnemy.UpdateIntentVisual();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateUI()
    {
        int i = 1;
        foreach(TextureRect heart in m_Hearts)
        {
            heart.Visible = i <= m_PlayerLife;
            i++;
        }

        if (m_TargetBattleScore == 0)
        {
            m_ScoreBar.Visible = false;
        }
        else
        {
            m_ScoreBar.Visible = true;
            m_ScoreBar.Value = m_CurrentBattleScore * 100 / m_TargetBattleScore; 
            m_LimitBar.Visible = m_CurrentBattleScore < m_CurrentLifeLimit;
            Vector2 LimitPos = new Vector2(0, 960.0f - ((float)m_CurrentLifeLimit * 960.0f / (float)m_TargetBattleScore));
            m_LimitBar.SetPosition(LimitPos);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public bool IsPlayerTurn()
    {
        return m_StateMachine.IsCurrentState(State_PlayerTurn);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public bool IsEnemyTurn()
    {
        return m_StateMachine.IsCurrentState(State_EnemyTurn);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void EndTurn()
    {
        if (IsPlayerTurn())
        {
            m_StateMachine.SetCurrentStateFunction(State_EnemyTurn);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public bool CheckEndFight()
    {
        if (m_ActiveEnemy.m_BattleTarget <= m_CurrentBattleScore)
        {
            return true;
        }

        return false;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void EndFight()
    {
        GameManager.GetManager().EndBattle();
        m_StateMachine.SetCurrentStateFunction(State_EndBattle);

        m_ActiveEnemy.QueueFree();
        m_ActiveEnemy = null;
    }
    

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void UpdateCardHandPosition()
    {
        int cardCount = m_CardHand.Count;
        for(int i = 0; i < cardCount; i++)
        {
            Card card = m_CardHand[i];
            float cardOffset = i * 125.0f - (cardCount - 1) * 72.5f;
            card.Position = m_HandNode.Position + new Vector2(0.0f, cardOffset);

            card.ZIndex = i;
            if (card == m_CurrentHeldCard)
            {
                card.ZIndex = 10;
            }
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ShuffleDeck()
    { 
        RandomManager.RandomizeList(ref m_CardDeck);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void DrawCard(int count)
    {
        while (count > 0 && (m_CardDeck.Count > 0 || m_CardDiscard.Count > 0))
        {
            if (m_CardDeck.Count == 0)
            {
                ReshuffleDiscardInDeck(true);
            }
            count--;
            Card card = m_CardDeck[0];
            m_CardDeck.RemoveAt(0);
            m_CardHand.Add(card);
        }
        
        //GD.Print("CardDeckCount : " + m_CardDeck.Count);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ReshuffleDiscardInDeck(bool shuffle)
    {
        while (m_CardDiscard.Count > 0)
        {
            m_CardDeck.Add(m_CardDiscard[0]);
            m_CardDiscard.RemoveAt(0);
        }

        if (shuffle)
        {
            ShuffleDeck();
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void PickCard(Card card)
    {
        if (m_CurrentMana >= card.m_ManaCost)
        {
            m_CurrentHeldCard = card;
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void DropCard()
    {
        m_CurrentHeldCard = null;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void PayManaCost(int cost)
    {
        m_CurrentMana -= cost;
        m_ManaLabel.Text = m_CurrentMana.ToString();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void DiscardAll()
    {
        while (m_CardHand.Count > 0)
        {
            Discard(m_CardHand[0]);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void CheckForExhaust()
    {
        _CheckForExhaust(ref m_CardDeck);
        _CheckForExhaust(ref m_CardHand);
        _CheckForExhaust(ref m_CardDiscard);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _CheckForExhaust(ref List<Card> cardList)
    {
        for(int i = cardList.Count - 1; i >= 0; i--)
        {
            Card card = cardList[i];
            if (card.m_MarkedForExhaust)
            {
                card.QueueFree();
                cardList.Remove(card);
            }
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ClearGameDeck()
    {
        Debug.Assert(m_CardHand.Count == 0, "Clear Game Deck with Hand not empty");
        Debug.Assert(m_CardDiscard.Count == 0, "Clear Game Deck with Discard not empty");

        // This is bad, because card generated will still be there, and card modified will stay modified.
        foreach(Card card in m_CardDeck)
        {
            card.Position = m_DeckNode.Position;
            card.QueueFree();
        }
        m_CardDeck.Clear();
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void CopyRunDeckToGameDeck()
    {
        foreach(CardData cardData in CardManager.GetManager().m_RunDeck)
        {
            Card newCard = Card.CreateCardFromCardData(cardData);
            AddChild(newCard);
            m_CardDeck.Add(newCard);
            newCard.Position = m_DeckNode.Position;
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void Discard(Card card)
    {
        m_CardHand.Remove(card);
        m_CardDiscard.Add(card);
        card.Position = m_DiscardNode.Position;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ResetManaToMax()
    {
        m_CurrentMana = m_MaxMana;
        m_ManaLabel.Text = m_CurrentMana.ToString();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void AddCardToZone(Card card, CardZone zone)
    {
        AddChild(card);
        card.Position = m_DiscardNode.Position;
        switch(zone)
        {
            case CardZone.Deck: m_CardDeck.Add(card); ShuffleDeck(); break;
            case CardZone.Hand: m_CardHand.Add(card); break;
            case CardZone.Discard: m_CardDiscard.Add(card); break;
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateArrow()
    {
        if (m_CurrentHeldCard == null)
        {
            m_CardArrow.Hide();
        }
        else
        {
            m_CardArrow.Show();
            Vector2 cardPosition = m_CurrentHeldCard.GlobalPosition;
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 middlePointPosition = new Vector2(cardPosition.X + 50.0f, mousePosition.Y - 50.0f);
            m_CardArrow.Position = Vector2.Zero;
            m_CardArrow.SetPointPosition(0, cardPosition);
            m_CardArrow.SetPointPosition(10, mousePosition);

            for(int i = 1; i < 10; i++)
            {
                Vector2 segmentAPos = cardPosition.Lerp(middlePointPosition, (float)i / (float)10);
                Vector2 segmentBPos = middlePointPosition.Lerp(mousePosition, (float)i / (float)10);
                m_CardArrow.SetPointPosition(i, segmentAPos.Lerp(segmentBPos, (float)i / (float)10));
            }
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void AddModifier(BaseModifierData modifierData)
    {
        ModifierDisplay newDisplay = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/ModifierDisplay.tscn").Instantiate<ModifierDisplay>();
        m_ModifierNode.AddChild(newDisplay);

        BaseModifier newModifier = modifierData.CreateModifierFromData(newDisplay);
        m_Modifiers.Add(newModifier);
        _UpdateModifiersPosition();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void RemoveModifier(BaseModifier modifier)
    {
        m_Modifiers.Remove(modifier);
        modifier.Clear();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void RemoveAllModifiers()
    {
        Vector2 position = Vector2.Zero;
        while(m_Modifiers.Count > 0)
        {
            RemoveModifier(m_Modifiers[0]);
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _UpdateModifiersPosition()
    {
        Vector2 position = Vector2.Zero;
        foreach(BaseModifier modifier in m_Modifiers)
        {
            modifier.UpdateModifierDisplayPosition(position);
            position += Vector2.Right * 50.0f;
        }
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public StateFunc State_StartBattle(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter: 
            {
                CopyRunDeckToGameDeck();
                ShuffleDeck();
                UpdateLimit();
                break;
            }
            case StateFunctionCall.Update: 
            {
                return State_PlayerTurn;
            }
            case StateFunctionCall.Exit: 
            {
                DiscardAll();
                break;
            }
        }
        return null;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public StateFunc State_PlayerTurn(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter: 
            {
                ElementBoard.GetBoard().SetStateToMoveElement();
                ResetManaToMax();
                DrawCard(5);
                break;
            }
            case StateFunctionCall.Update: 
            {
                // N/A
                break;
            }
            case StateFunctionCall.Exit: 
            {
                DiscardAll();
                break;
            }
        }
        return null;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public StateFunc State_EnemyTurn(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter: 
            {
                EmitSignal(SignalName.OnTurnEnd);
                ElementBoard.GetBoard().ForceCheckBoardForMatch();
                break;
            }
            case StateFunctionCall.Update: 
            {
                if (ElementBoard.GetBoard().IsBoardIdle())
                {
                    ApplyEnemyAttack();

                    if (m_PlayerLife > 0)
                    {
                        return State_PlayerTurn;
                    }
                    else
                    {
                        return State_GAMEOVER;
                    }
                }
                break;
            }
            case StateFunctionCall.Exit: 
            {
                // N/A
                break;
            }
        }
        return null;
    }
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public StateFunc State_EndBattle(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter: 
            {
                GD.Print("End of battle !");
                m_TargetBattleScore = 0;
                DiscardAll();
                ReshuffleDiscardInDeck(false);
                ClearGameDeck();
                RemoveAllModifiers();
                GameManager.GetManager().EndBattle();
                break;
            }
            case StateFunctionCall.Update: 
            {
                // N/A
                break;
            }
            case StateFunctionCall.Exit: 
            {
                // N/A
                break;
            }
        }
        return null;
    }

    
    
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public StateFunc State_GAMEOVER(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter: 
            {
                m_GameOverRect.Visible = true;
                break;
            }
            case StateFunctionCall.Update: 
            {
                // N/A
                break;
            }
            case StateFunctionCall.Exit: 
            {
                // N/A
                break;
            }
        }
        return null;
    }
}
