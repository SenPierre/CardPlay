using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseTreasureEffect : Resource
{   
    public abstract void Connect();
    public abstract void Disconnect();
    public virtual void OnPickup() {}
}