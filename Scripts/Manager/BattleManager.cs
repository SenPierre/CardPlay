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
	
	[Export] public Label m_ButtonLabel;

	[Export] public int m_MaxMana = 3;

	[Export] public SubViewport m_ElementBoardMaskViewport;

	private Enemy m_ActiveEnemy = null;
	private int m_CurrentBattleScore = 0;
	private int m_TargetBattleScore = 0;
	private int m_CurrentLifeLimit = 0;
	private int m_PlayerLife = 3;

	private bool m_AwaitingPlayerInput = false;
	private bool m_IsEndOfTurn = false;
	
	private StateQueue m_StateQueue = new StateQueue();
	
	List<Card> m_CardDeck = new List<Card>();
	List<Card> m_CardHand = new List<Card>();
	List<Card> m_CardDiscard = new List<Card>();

	public Card m_CurrentHeldCard;

	List<BaseModifier> m_Modifiers = new List<BaseModifier>();

	int m_CurrentMana = 0;

	private float m_CardAnimTimer = 0.0f;
	private bool m_StartingTurn = false;
	
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
	public void InitFight(Enemy newEnemy)
	{
		g_Manager = this;
		
		SetPlayerLife(3);
		m_GameOverRect.Visible = false;
		m_CardArrow.DefaultColor = m_HintColor;
		
		m_ActiveEnemy = newEnemy;
		m_EnemyRoot.AddChild(m_ActiveEnemy);        
		//m_ActiveEnemy.UpdateIntentVisual();
		ResetBattle();
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void ResetBattle()
	{
		if (m_ActiveEnemy.IsAPuzzle() == false)
		{
			SetupBattle();
		}
		else
		{
			m_ButtonLabel.Text = "Reset";
			CreatePuzzleSituation();
		}
		StartTurn();
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void SetupBattle()
	{
		ElementBoard.GetBoard().InitBoard(m_ActiveEnemy.m_BoardLayoutRes);
		CopyRunDeckToGameDeck();
		ShuffleDeck();
		UpdateLimit();
		m_TargetBattleScore = m_ActiveEnemy.m_BattleTarget;
		m_CurrentBattleScore = 0;
		
		UpdateUI();
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void CreatePuzzleSituation()
	{
		Puzzle puzzle = m_ActiveEnemy as Puzzle;
		if (puzzle.m_PuzzleDeck != null)
		{
			foreach(CardData cardData in puzzle.m_PuzzleDeck.m_Cards)
			{
				Card newCard = Card.CreateCardFromCardData(cardData);
				AddChild(newCard);
				m_CardDeck.Add(newCard);
				newCard.Position = m_DeckNode.Position;
			}
		}
		
		if (puzzle.m_PuzzleHand != null)
		{
			foreach(CardData cardData in puzzle.m_PuzzleHand.m_Cards)
			{
				Card newCard = Card.CreateCardFromCardData(cardData);
				AddChild(newCard);
				m_CardHand.Add(newCard);
			}
		}
		UpdateCardHandPosition();

		if (puzzle.m_PuzzleDiscard != null)
		{
			foreach(CardData cardData in puzzle.m_PuzzleDiscard.m_Cards)
			{
				Card newCard = Card.CreateCardFromCardData(cardData);
				AddChild(newCard);
				m_CardDiscard.Add(newCard);
				newCard.Position = m_DeckNode.Position;
			}
		}

		m_MaxMana = puzzle.m_StartingMana;
		m_CurrentMana = m_MaxMana;
		m_ManaLabel.Text = m_CurrentMana.ToString();

		m_TargetBattleScore = m_ActiveEnemy.m_BattleTarget;
		m_CurrentBattleScore = 0;

		ElementBoard.GetBoard().m_PuzzleMode = true;
		ElementBoard.GetBoard().ClearBoard();
		ElementBoard.GetBoard().InitBoard(m_ActiveEnemy.m_BoardLayoutRes);
		
		UpdateUI();
	}

	// =================================================================
	// 
	// =================================================================
	public override void _Process(double delta)
	{
		QueueRedraw();
		m_StateQueue.UpdateQueue();
		UpdateCardHandPosition();
		UpdateArrow();

		base._Process(delta);
	}

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Draw()
    {
        base._Draw();
#if TOOLS
        m_StateQueue.DebugDraw(this);

#endif
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
		//m_ActiveEnemy.UpdateIntentVisual();
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
	public void StartTurn()
	{
		m_StartingTurn = true;
		ResetManaToMax();
		DrawCard(5);
		EventManager.GetManager().EmitSignal(EventManager.SignalName.OnTurnStart);
		AddToStateQueue(Queue_AwaitingPlayerInput, -100);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void AwaitForInput()
	{
		AddToStateQueue(Queue_AwaitingPlayerInput, -100);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool IsAwaitingPlayerInput()
	{
		return m_StateQueue.IsCurrentState(Queue_AwaitingPlayerInput);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool IsEndOfTurn()
	{
		return m_IsEndOfTurn;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void ButtonEndTurnPressed()
	{
		if (IsAwaitingPlayerInput())
		{
			EndTurnDiscard();
			EventManager.GetManager().EmitSignal(EventManager.SignalName.OnTurnEnd);
			AddToStateQueue(Queue_TurnEnding, -10);
			m_AwaitingPlayerInput = false;
			m_IsEndOfTurn = true;
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
		AddToStateQueue(Queue_EndBattle, -50);
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
			count--;
			AddToStateQueue(Queue_DrawCard, m_StartingTurn ? 0 : 50);
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
		if (IsAwaitingPlayerInput() || HasACardSelected())
		{
			if (m_CurrentMana >= card.m_ManaCost)
			{
				if (m_CurrentHeldCard != null)
				{
					m_CurrentHeldCard.Dropped();
				}

				m_CurrentHeldCard = card;
				m_AwaitingPlayerInput = false;
				m_CurrentHeldCard.Selected();
			}
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void DropCard()
	{
		m_CurrentHeldCard.Dropped();
		m_CurrentHeldCard = null;
		m_AwaitingPlayerInput = false;
		AddToStateQueue(Queue_AwaitingPlayerInput, -100);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool HasACardSelected()
	{
		return m_CurrentHeldCard != null;
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
	public int GetHandCount()
	{
		return m_CardHand.Count;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void DiscardAll(bool exceptSelected)
	{
		for (int i = m_CardHand.Count - 1; i >= 0; i--)
		{
			Card card = m_CardHand[i];
			if (exceptSelected == false || card != m_CurrentHeldCard)
			{
				card.Discard(false);
			}
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void EndTurnDiscard()
	{
		for (int i = m_CardHand.Count - 1; i >= 0; i--)
		{
			Card card = m_CardHand[i];
			card.Discard(true);
		}
		CheckForExhaust();
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
	public void ClearCards()
	{
		// This is bad, because card generated will still be there, and card modified will stay modified.
		foreach(Card card in m_CardDeck)
		{
			card.Position = m_DeckNode.Position;
			card.QueueFree();
		}
		foreach(Card card in m_CardHand)
		{
			card.Position = m_DeckNode.Position;
			card.QueueFree();
		}
		foreach(Card card in m_CardDiscard)
		{
			card.Position = m_DeckNode.Position;
			card.QueueFree();
		}
		m_CardDeck.Clear();
		m_CardHand.Clear();
		m_CardDiscard.Clear();
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
	public void AddToStateQueue(QueueFunc func, int priority)
	{
		m_StateQueue.AddToTheQueue(func, priority);
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_AwaitingPlayerInput(QueueFuncCall a_Call)
	{
		switch (a_Call)
		{
			case QueueFuncCall.Activation : 
			{
				m_StartingTurn = false;
				ElementBoard.GetBoard().ResetMultiplier();
				m_AwaitingPlayerInput = true;
				return false;
			}
			case QueueFuncCall.Update :
			{
				return m_AwaitingPlayerInput == false;
			}
		}
		return false;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_DrawCard(QueueFuncCall a_Call)
	{
		switch (a_Call)
		{
			case QueueFuncCall.Activation : 
			{
				if (m_CardDeck.Count == 0)
				{
					ReshuffleDiscardInDeck(true);
					if (m_CardDeck.Count == 0)
					{
						// Deck and discard empty !!
						return true;
					}
				}
				Card card = m_CardDeck[0];
				m_CardDeck.RemoveAt(0);
				m_CardHand.Add(card);
				card.OnDraw();
				m_CardAnimTimer = 0.0f;
				break;
			}
			case QueueFuncCall.Update :
			{
				m_CardAnimTimer += TimeManager.GetDeltaTime();
				if (m_CardAnimTimer > 0.1f)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_TurnEnding(QueueFuncCall a_Call)
	{
		if (m_ActiveEnemy.IsAPuzzle() == false)
		{
			ElementBoard.GetBoard().ForceCheckBoardForMatch();
			AddToStateQueue(Queue_EnemyTurn, 0);
		}
		else
		{
			ClearCards();
			DamagePlayer();
			ResetBattle();
		}
		m_IsEndOfTurn = false;
		return true;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_EnemyTurn(QueueFuncCall a_Call)
	{
		switch (a_Call)
		{
			case QueueFuncCall.Activation : 
			{
				ApplyEnemyAttack();

				if (m_PlayerLife > 0)
				{
					StartTurn();
				}
				else
				{
					AddToStateQueue(Queue_GAMEOVER, 0);
				}
				return true;
			}
			case QueueFuncCall.Update :
			{
				return true;
			}
		}
		return false;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_EndBattle(QueueFuncCall a_Call)
	{
		switch (a_Call)
		{
			case QueueFuncCall.Activation: 
			{
				m_ActiveEnemy.QueueFree();
				m_ActiveEnemy = null;
				GD.Print("End of battle !");
				m_TargetBattleScore = 0;
				ClearCards();
				RemoveAllModifiers();
				GameManager.GetManager().EndBattle();
				break;
			}
			case QueueFuncCall.Update: 
			{
				// N/A
				break;
			}
		}
		// End of battle, never leeeeave
		return false;
	}

	
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool Queue_GAMEOVER(QueueFuncCall a_Call)
	{
		switch (a_Call)
		{
			case QueueFuncCall.Activation: 
			{
				m_GameOverRect.Visible = true;
				break;
			}
			case QueueFuncCall.Update: 
			{
				// N/A
				break;
			}
		}
		return false;
	}
}
