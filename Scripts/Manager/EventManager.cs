using Godot;
using System;
using System.Collections.Generic;

public partial class EventManager : Node2D
{
	// STATIC PART ----------------------------
	static EventManager g_Manager;

	static public EventManager GetManager()
	{
		return g_Manager;
	} 
	// STATIC PART ----------------------------

	[Signal]
	public delegate void OnBattleStartEventHandler();
	[Signal]
	public delegate void OnTurnStartEventHandler();
	[Signal]
	public delegate void OnTurnEndEventHandler();
	[Signal]
	public delegate void OnHealthLosseEventHandler();
	[Signal]
	public delegate void OnMatchesEventHandler();
	[Signal]
	public delegate void OnIndividualDestroyEventHandler();
	[Signal]
	public delegate void OnIndividualMatchEventHandler();
	[Signal]
	public delegate void OnCardPlayedEventHandler();
	[Signal]
	public delegate void OnCardDiscardedEventHandler();
	[Signal]
	public delegate void OnCardExhaustEventHandler();
	[Signal]
	public delegate void OnDeckShuffledEventHandler();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public override void _Ready()
	{
		g_Manager = this;
		base._Ready();
	}
}
