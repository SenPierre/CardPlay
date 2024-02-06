using Godot;
using System;

[Tool]
public partial class ElementShaderVariablesHandler : Sprite2D
{
	private float _whiteness;
	[Export] public float whiteness {
		get { return _whiteness;}
		set { _whiteness = value; GetShaderMaterial().SetShaderParameter("whiteness", _whiteness); }
	}
	private float _blackness;
	[Export] public float blackness {
		get { return _blackness;}
		set { _blackness = value; GetShaderMaterial().SetShaderParameter("blackness", _blackness); }
	}
	private float _destroying;
	[Export] public float destroying {
		get { return _destroying;}
		set { _destroying = value; GetShaderMaterial().SetShaderParameter("destroying", _destroying); }
	}
	private float _alphaModulate;
	[Export] public float alphaModulate {
		get { return _alphaModulate;}
		set { _alphaModulate = value; GetShaderMaterial().SetShaderParameter("alphaModulate", _alphaModulate); }
	}

	ElementShaderVariablesHandler()
	{
		whiteness = 0;
		alphaModulate = 0;
	}

	ShaderMaterial GetShaderMaterial()
	{
		return Material as ShaderMaterial;
	}
}
