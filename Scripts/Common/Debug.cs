using System;
using Godot;

internal static class Debug
{
	internal static void Assert(bool cond, string msg)
#if TOOLS
{
	if (cond) return;

	GD.PrintErr(msg);
	throw new ApplicationException($"Assert Failed: {msg}");
}
#else
	{ }
#endif
}
