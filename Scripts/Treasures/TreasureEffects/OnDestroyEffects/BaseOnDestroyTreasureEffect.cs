using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseOnDestroyTreasureEffect : BaseTreasureEffect
{
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Connect()
    {
        EventManager.GetManager().OnIndividualDestroy += _OnDestroy;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void Disconnect()
    {
        EventManager.GetManager().OnIndividualDestroy -= _OnDestroy;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _OnDestroy()
    {
        OnDestroy();
    }

    public abstract void OnDestroy();
}