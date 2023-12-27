using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EnemyData : Resource
{
   [Export] public int m_BattleTarget = 2500;
   [Export] public int m_LimitIncrement = 100;
   [Export] public BaseEnemyIntent[] m_EnemyIntents;
   [Export] public string m_BoardLayoutRes;
}
