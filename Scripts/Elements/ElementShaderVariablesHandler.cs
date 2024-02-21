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
	private float _shiningLerp;
	[Export] public float shiningLerp {
		get { return _shiningLerp;}
		set { _shiningLerp = value; GetShaderMaterial().SetShaderParameter("shiningLerp", _shiningLerp); }
	}

	public bool m_CanShine = false;
	private bool m_IsLerpingShine = false;

	ElementShaderVariablesHandler()
	{
		whiteness = 0;
		alphaModulate = 0;
		shiningLerp = 0.0f;
		m_IsLerpingShine = false;
	}

	ShaderMaterial GetShaderMaterial()
	{
		return Material as ShaderMaterial;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (m_CanShine && RandomManager.GetIntRange(0, 20000) == 0)
		{
			m_IsLerpingShine = true;
		}

		if (m_IsLerpingShine)
		{
			shiningLerp += TimeManager.GetDeltaTime() * 2.0f;
			if (shiningLerp >= 1.0)
			{
				shiningLerp = 0.0f;
				m_IsLerpingShine = false;
			} 
		}
    }
}
