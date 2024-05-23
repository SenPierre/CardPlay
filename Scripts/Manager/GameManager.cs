using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	// STATIC PART ----------------------------
	static GameManager g_Manager;

	static public GameManager GetManager()
	{
		return g_Manager;
	} 
	// STATIC PART ----------------------------

	[Export] public PackedScene m_BattleScene;
	[Export] public PackedScene m_CardRewardScene;
	[Export] public PackedScene m_MapScene;

	[Export] public EnemyData m_GrandmaData;
	[Export] public EnemyDataPool[] m_EnemyPool;

	public CardDataBase m_CardDatabase = new CardDataBase();
	public ElementDataBase m_ElementDatabase = new ElementDataBase();
	public StateMachine m_StateMachine = new StateMachine();

	public Node2D m_CurrentSubScene = null;

	private MapData m_CurrentMapData = null;
	private int m_currentMapIndex = 0;
	private int m_nextCardRewardCount = 2;

	private List<int> m_AlreadyDidEnemy = new List<int>();
	private List<int> m_AlreadyDidPuzzle = new List<int>();

	private EnemyData m_NextEnemy = null;
	private PuzzleData m_NextPuzzle = null;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public override void _EnterTree()
	{
		m_CardDatabase.Init();
		m_ElementDatabase.Init();
		base._EnterTree();
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public override void _Ready()
	{
		g_Manager = this;
		base._Ready();
		m_StateMachine.SetCurrentStateFunction(State_Map);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public override void _Process(double delta)
	{
		m_StateMachine.UpdateStateMachine();
		base._Process(delta);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void MapNodeSelected(MapNode node)
	{
		switch(node.m_NodeDatas.m_Type)
		{
			case MapNodeDataType.Fight: StartNormalFight(); break;
			case MapNodeDataType.Grandma: StartGrandmaFight(); break;
			case MapNodeDataType.Puzzle: StartPuzzle(); break;
			case MapNodeDataType.Card: StartCardReward(2); break;
			
		}
		m_currentMapIndex = node.m_NodeDatas.m_Index;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void StartGrandmaFight()
	{
		m_NextEnemy = m_GrandmaData;
		m_StateMachine.SetCurrentStateFunction(State_Battle);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void StartNormalFight()
	{
		int newEnemyIndex = 0;

		Debug.Assert(m_EnemyPool[0].m_EnemyData.Length != m_AlreadyDidEnemy.Count, "No more enemy");
		

		do {
			newEnemyIndex = RandomManager.GetIntRange(0, m_EnemyPool[0].m_EnemyData.Length - 1);
		} while (m_AlreadyDidEnemy.Contains(newEnemyIndex));

		m_AlreadyDidEnemy.Add(newEnemyIndex);

		m_NextEnemy = m_EnemyPool[0].m_EnemyData[newEnemyIndex];
		m_StateMachine.SetCurrentStateFunction(State_Battle);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void StartPuzzle()
	{
		int newPuzzleIndex = 0;

		Debug.Assert(m_EnemyPool[0].m_PuzzleData.Length != m_AlreadyDidEnemy.Count, "No more puzzle");

		do {
			newPuzzleIndex = RandomManager.GetIntRange(0, m_EnemyPool[0].m_PuzzleData.Length - 1);
		} while (m_AlreadyDidEnemy.Contains(newPuzzleIndex));

		m_AlreadyDidPuzzle.Add(newPuzzleIndex);

		m_NextPuzzle = m_EnemyPool[0].m_PuzzleData[newPuzzleIndex];
		m_StateMachine.SetCurrentStateFunction(State_Puzzle);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void EndBattle()
	{
		StartCardReward(4);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void StartCardReward(int cardCount)
	{
		m_nextCardRewardCount = cardCount;
		m_StateMachine.SetCurrentStateFunction(State_AddCard);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void EndReward()
	{
		m_StateMachine.SetCurrentStateFunction(State_Map);
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void SelectingACard(Card card)
	{
		if (m_StateMachine.IsCurrentState(State_Battle) || m_StateMachine.IsCurrentState(State_Puzzle))
		{
			BattleManager.GetManager().PickCard(card);
		}
		else if (m_StateMachine.IsCurrentState(State_AddCard))
		{
			CardRewardManager.GetManager().SelectCard(card);
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void InitSubScene(Node2D newSubScene)
	{
		m_CurrentSubScene = newSubScene;
		AddChild(newSubScene);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void ClearSubScene()
	{
		RemoveChild(m_CurrentSubScene);
		m_CurrentSubScene.QueueFree();
		m_CurrentSubScene = null;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private StateFunc State_Battle(StateFunctionCall a_Call)
	{
		switch (a_Call)
		{
			case StateFunctionCall.Enter: 
			{
				BattleManager manager = m_BattleScene.Instantiate<BattleManager>();
				InitSubScene(manager);
				Enemy newEnemy = Enemy.CreateEnemyFromData(m_NextEnemy);
				manager.InitFight(newEnemy);
				break;
			}
			case StateFunctionCall.Update: 
			{
				// N/A
				break;
			}
			case StateFunctionCall.Exit: 
			{
				ClearSubScene();
				break;
			}
		}
		return null;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private StateFunc State_Puzzle(StateFunctionCall a_Call)
	{
		switch (a_Call)
		{
			case StateFunctionCall.Enter: 
			{
				BattleManager manager = m_BattleScene.Instantiate<BattleManager>();
				InitSubScene(manager);
				Enemy newPuzzle = Puzzle.CreatePuzzleFromData(m_NextPuzzle);
				manager.InitFight(newPuzzle);
				break;
			}
			case StateFunctionCall.Update: 
			{
				// N/A
				break;
			}
			case StateFunctionCall.Exit: 
			{
				ClearSubScene();
				break;
			}
		}
		return null;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private StateFunc State_AddCard(StateFunctionCall a_Call)
	{
		switch (a_Call)
		{
			case StateFunctionCall.Enter: 
			{
				CardRewardManager rewardManager = m_CardRewardScene.Instantiate<CardRewardManager>();
				rewardManager.m_displayedCardCount = m_nextCardRewardCount;

				InitSubScene(rewardManager);
				rewardManager.Init();
				break;
			}
			case StateFunctionCall.Update: 
			{
				// N/A
				break;
			}
			case StateFunctionCall.Exit: 
			{
				ClearSubScene();
				break;
			}
		}
		return null;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private StateFunc State_Map(StateFunctionCall a_Call)
	{
		switch (a_Call)
		{
			case StateFunctionCall.Enter: 
			{
				MapManager mapManager = m_MapScene.Instantiate<MapManager>();
				
				InitSubScene(mapManager);
				
				if (m_CurrentMapData == null)
				{
					m_CurrentMapData = MapManager.GenerateMap(false /* has grandma */, 10, 2, 3);
				}

				mapManager.Init(m_CurrentMapData);
				mapManager.SetCurrentNodeIndex(m_currentMapIndex);

				break;
			}
			case StateFunctionCall.Update: 
			{
				// N/A
				break;
			}
			case StateFunctionCall.Exit: 
			{
				ClearSubScene();
				break;
			}
		}
		return null;
	}
}
