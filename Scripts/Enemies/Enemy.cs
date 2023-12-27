using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Enemy : Node
{
    [Export] public Sprite2D m_ShieldSprite;
    [Export] public Sprite2D m_BuffSprite;
    [Export] public Sprite2D m_DebuffSprite;

    public int m_BattleTarget = 2500;
    public int m_LimitIncrement = 100;
    public string m_BoardLayoutRes;

    public BaseEnemyIntent[] m_EnemyIntent;

    private int m_currentIntent = 0;
    private int m_LimitCount = 0;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    static public Enemy CreateEnemyFromData(EnemyData data)
    {
        Enemy newEnemy = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Enemy.tscn").Instantiate<Enemy>();

        newEnemy.m_BattleTarget = data.m_BattleTarget;

        newEnemy.m_EnemyIntent = data.m_EnemyIntents;
        newEnemy.m_LimitIncrement = data.m_LimitIncrement;
        newEnemy.m_BoardLayoutRes = data.m_BoardLayoutRes;

        newEnemy.m_currentIntent = 0;

        return newEnemy;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void ApplyCurrentIntent()
    {
        m_EnemyIntent[m_currentIntent++].ApplyIntent(this);

        if (m_currentIntent >= m_EnemyIntent.GetLength(0))
        {
            m_currentIntent = 0;
        }
        m_LimitCount++;
        BattleManager.GetManager().UpdateLimit();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void UpdateIntentVisual()
    {
        m_ShieldSprite.Visible = false;
        m_ShieldSprite.Position = Vector2.Zero;
        //m_BuffSprite.Visible = false;
        //m_DebuffSprite.Visible = false;
        m_EnemyIntent[m_currentIntent].ShowIntent(this, Vector2.Zero);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public int GetLimit()
    {
        return m_EnemyIntent[m_currentIntent].GetLimitBase() + m_LimitIncrement * m_LimitCount;
    }
}
