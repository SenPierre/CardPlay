using Godot;
using System;

[GlobalClass]
public abstract partial class BaseEnemyIntent : Resource
{
   [Export] public int m_LimitBase = 500;

    public abstract void ApplyIntent(Enemy enemy);
    public abstract void ShowIntent(Enemy enemy, Vector2 m_Offset);

    public virtual int GetLimitBase() { return m_LimitBase; }
}
