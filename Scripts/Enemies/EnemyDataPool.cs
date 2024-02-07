using Godot;
using System;
using System.Collections.Generic;
using System.IO;

[GlobalClass]
public partial class EnemyDataPool : Resource
{
    [Export] public EnemyData[] m_EnemyData = new EnemyData[0];
    [Export] public EnemyData[] m_EliteData = new EnemyData[0];
    [Export] public EnemyData[] m_BossData = new EnemyData[0];
    
    [Export] public PuzzleData[] m_PuzzleData = new PuzzleData[0];
}
