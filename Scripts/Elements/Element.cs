using Godot;
using System;
using System.Collections.Generic;

public enum ElementMovementAnimation {
	Default,
	Gravity,
	RotateClockwise,
	RotateAntiClockwise,
	FadeOutFadeIn
}

//##########################################################################################################
//
//##########################################################################################################
public class ElementsMatch
{
	public static ElementsMatch s_currentMatch = null;
    public ElementType m_Type;
    public List<Element> m_Element = new List<Element>();
    public Vector2 m_matchDirection;
	public int m_matchScore = 0;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void OnMatch(float multiplier)
	{
		s_currentMatch = this;
		int matchOverload = Math.Max(0, m_Element.Count - 4);
		int scoreOverload = 0;
		for (int i = 0; i < matchOverload; i++)
		{
			scoreOverload += scoreOverload + 100;
		}
		m_matchScore = (int)Math.Floor((400 + scoreOverload) * multiplier);
		
		EventManager.GetManager().EmitSignal(EventManager.SignalName.OnIndividualMatch);

		BattleManager.GetManager().AddToScore(m_matchScore);
		s_currentMatch = null;
	}
}

//##########################################################################################################
//
//##########################################################################################################
public partial class Element : Node2D
{
	static public float ElementSize = 80.0f;
	static public float ElementHalfSize = 40.0f;
	static public Element s_LastDestroyedElement = null;

	[Export] ElementShaderVariablesHandler m_Sprite;
	[Export] AnimationPlayer m_Anim;
	public ElementType m_Type = ElementType.Void;
	public bool m_ToDelete;

	private ElementData m_data = null;

	private BaseElementBehavior m_Behavior = null;

	private bool m_IsMoving = false;
	private bool m_IsTransforming = false;

	private Vector2 m_StartPos;
	private float m_StartAngle;
	private float m_StartSquareRadius;
	
	private Vector2 m_TargetPos;
	private float m_TargetAngle;
	private float m_TargetSquareRadius;

	private float m_gravitySpeed = 0.0f;

	private Vector2 m_RotationCenter;

	private ElementBoard m_Board;
	private ElementMovementAnimation m_Animation;

	private Dictionary<Vector2, ElementsMatch> m_Matches = new Dictionary<Vector2, ElementsMatch>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public static Element CreateElementFromType(ElementType element)
	{
		Element newEl = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Elements/ElementBase.tscn").Instantiate<Element>();
		newEl._SetElement(element);
		return newEl;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public override void _Ready()
	{

	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void Init(ElementBoard board, Vector2 startPos, Vector2 targetPos)
	{
		(m_Sprite.Material as ShaderMaterial).SetShaderParameter("mask", BattleManager.GetManager().m_ElementBoardMaskViewport.GetTexture());

		m_Board = board;
		Position = startPos;
		m_StartPos = startPos;
		m_TargetPos = targetPos;
		m_IsMoving = true;
		m_Animation = ElementMovementAnimation.Gravity;
		m_RotationCenter = board.m_MiddleOfTheBoard;
	}

	// -----------------------------------------------------------------
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// -----------------------------------------------------------------
	public override void _Process(double delta)
	{
		if (IsMoving())
		{
			switch (m_Animation)
			{
				case ElementMovementAnimation.Default: _MoveDefault(); break;
				case ElementMovementAnimation.Gravity: _MoveGravity(); break;
				case ElementMovementAnimation.RotateClockwise: _MoveRotate(); break;
				case ElementMovementAnimation.RotateAntiClockwise: _MoveRotate();break;
				case ElementMovementAnimation.FadeOutFadeIn: _MoveFadeOutFadeIn(); break;
			}
		}

		base._Process(delta);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void TransformElement(ElementType element)
	{
		m_Type = element;
		m_Anim.Play("Transforming");
		m_IsTransforming = true;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void ConfirmTransform()
	{
		_SetElement(m_Type);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void EndTransform()
	{
		m_IsTransforming = false;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void _SetElement(ElementType element)
	{
		m_Type = element;
		
		if (m_Behavior != null)
		{
			m_Behavior.ClearBehavior();
			m_Behavior.QueueFree();
		}

		ElementDataBase db = GameManager.GetManager().m_ElementDatabase;
		m_data = db.GetDataFromType(element);

		m_Sprite.Texture = m_data.m_Sprite;
		m_Sprite.m_CanShine = element >= ElementType.Element1 && element <= ElementType.Element4;

		if (m_data.m_ElementBehaviorNode != null)
		{
			m_Behavior = m_data.m_ElementBehaviorNode.Instantiate<BaseElementBehavior>();
			AddChild(m_Behavior);
			m_Behavior.Position = Vector2.Zero;
			m_Behavior.InitBehavior(this);
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CheckElement(ElementType type)
	{
		if (type == ElementType._Count)
		{
			return true;
		}

		if (type == ElementType.BasicElement)
		{
			return m_Type >= ElementType.Element1 && m_Type <= ElementType.Element4;
		}

		return m_Type == type;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool IsIdle()
	{
		return (m_IsMoving == false) && (m_IsTransforming == false);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void MoveElement(Vector2 newPos, ElementMovementAnimation animation)
	{
		m_IsMoving = true;
		m_StartPos = Position;
		m_TargetPos = newPos;
		m_gravitySpeed = 200.0f;

		m_Animation = animation;
		if (animation == ElementMovementAnimation.RotateClockwise || animation == ElementMovementAnimation.RotateAntiClockwise)
		{
			Vector2 startRadiusVector = GlobalPosition - m_RotationCenter;
			m_StartSquareRadius = startRadiusVector.LengthSquared();
			m_StartAngle = startRadiusVector.Angle();

			Vector2 endRadiusVector = m_TargetPos - Position + GlobalPosition - m_RotationCenter;
			m_TargetSquareRadius = endRadiusVector.LengthSquared();
			m_TargetAngle = endRadiusVector.Angle();
			
			if (animation == ElementMovementAnimation.RotateAntiClockwise)
			{
				while (m_StartAngle < m_TargetAngle)
				{
					m_TargetAngle -= Mathf.DegToRad(360.0f);
				}
			}
			else
			{
				while (m_StartAngle > m_TargetAngle)
				{
					m_TargetAngle += Mathf.DegToRad(360.0f);
				}
			}
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool IsMoving()
	{
		return m_IsMoving;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public ElementsMatch GetMatch(Vector2 directionMatch)
	{
		return m_Matches.ContainsKey(directionMatch) ? m_Matches[directionMatch] : null;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void SetMatch(Vector2 directionMatch, ElementsMatch match)
	{
		Debug.Assert(GetMatch(directionMatch) == null, "Set Match to an element which has already a match for the given direction.");
		m_Matches[directionMatch] = match;
		match.m_Element.Add(this);
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CanMatch()
	{
		return m_data.m_CanMatch;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CanBeDestroyed()
	{
		return m_data.m_CanBeDestroyed;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CanBeMoved()
	{
		return m_data.m_CanBeMoved;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CanBeTransformed()
	{
		return m_data.m_CanBeTransformed;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public bool CanFall()
	{
		return m_data.m_CanFall;
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void DestroyElement()
	{
		s_LastDestroyedElement = this;
		m_IsTransforming = true;
		if (m_Matches.Count > 0)
		{
			m_Anim.Play("Matching");
		}
		else
		{
			EventManager.GetManager().EmitSignal(EventManager.SignalName.OnIndividualDestroy);
			m_Anim.Play("Destroying");
		}
	}
	
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void RemoveFromBoard()
	{
		// Animation, VFX, this kind of thing. For now just remove it.
		if (m_Behavior != null)
		{
			m_Behavior.ClearBehavior();
			m_Behavior.QueueFree();
		}
		m_Board.RemoveElementFromBoard(this);
		QueueFree();
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void SetRotationCenter(Vector2 center)
	{
		m_RotationCenter = center;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	public void EndMove()
	{
		Position = m_TargetPos;
		m_IsMoving = false;
		m_RotationCenter = m_Board.m_MiddleOfTheBoard;
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void _MoveDefault()
	{
		Position = m_StartPos.Lerp(m_TargetPos, m_Board.m_MovingElementLerp);
		
		if (m_Board.m_MovingElementLerp >= 1.0)
		{
			EndMove();
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void _MoveGravity()
	{
		m_gravitySpeed += TimeManager.GetDeltaTime() * 1000.0f;
		Position = Position.MoveToward(m_TargetPos, m_gravitySpeed * TimeManager.GetDeltaTime());

		if (Position == m_TargetPos)
		{
			EndMove();
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void _MoveRotate()
	{
		float currentSquareRadius = Mathf.Lerp(m_StartSquareRadius, m_TargetSquareRadius, m_Board.m_MovingElementLerp);

		float currentAngle = Mathf.Lerp(m_StartAngle, m_TargetAngle, m_Board.m_MovingElementLerp);
		Vector2 newVectorRadius = Vector2.FromAngle(currentAngle) * Mathf.Sqrt(currentSquareRadius);
		GlobalPosition = m_RotationCenter + newVectorRadius;

		if (m_Board.m_MovingElementLerp >= 1.0)
		{
			EndMove();
		}
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
	private void _MoveFadeOutFadeIn()
	{
		float alphaModulate = 1.0f;
		if (m_Board.m_MovingElementLerp < 0.5f)
		{
			alphaModulate = Mathf.Lerp(1, 0, m_Board.m_MovingElementLerp * 2.0f);
		}
		else
		{
			Position = m_TargetPos;
			alphaModulate = Mathf.Lerp(0, 1, (m_Board.m_MovingElementLerp * 2.0f) - 1.0f);
		}

		(m_Sprite.Material as ShaderMaterial).SetShaderParameter("alphaModulate", alphaModulate);
		
		if (m_Board.m_MovingElementLerp >= 1.0)
		{
			EndMove();
		}
	}
}
