using Godot;
using System;
using System.Collections.Generic;

public enum ElementMovementAnimation {
    Default,
    RotateClockwise,
    RotateAntiClockwise,
    FadeOutFadeIn
}

//##########################################################################################################
//
//##########################################################################################################
public class ElementsMatch
{
    [Export] public Sprite2D m_element;
    public ElementType m_Type;
    public List<Element> m_Element = new List<Element>();
    public Vector2 m_matchDirection;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnMatch(float multiplier)
    {
        int matchOverload = Math.Max(0, m_Element.Count - 4);
        int scoreOverload = 0;
        for (int i = 0; i < matchOverload; i++)
        {
            scoreOverload += scoreOverload + 100;
        }
        BattleManager.GetManager().AddToScore((int)Math.Floor((400 + scoreOverload) * multiplier));
    }
}

//##########################################################################################################
//
//##########################################################################################################
public partial class Element : Node2D
{
    static public float ElementSize = 80.0f;
    static public float ElementHalfSize = 40.0f;

    [Signal]
    public delegate void ElementTransformedEventHandler();

    [Export] Sprite2D m_Sprite;
    public ElementType m_Type = ElementType.Void;
    public bool m_ToDelete;

    private ElementData m_data = null;

    private BaseElementBehavior m_Behavior = null;

    private bool m_IsMoving = false;

    private Vector2 m_StartPos;
    private float m_StartAngle;
    private float m_StartSquareRadius;
    
    private Vector2 m_TargetPos;
    private float m_TargetAngle;
    private float m_TargetSquareRadius;

    private ElementBoard m_Board;
    private ElementMovementAnimation m_Animation;

    private Dictionary<Vector2, ElementsMatch> m_Matches = new Dictionary<Vector2, ElementsMatch>();

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public static Element CreateElementFromType(ElementType element)
    {
        Element newEl = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Elements/ElementBase.tscn").Instantiate<Element>();
        newEl.SetElement(element);
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
        m_Board = board;
        Position = startPos;
        m_StartPos = startPos;
        m_TargetPos = targetPos;
        m_IsMoving = true;
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
                case ElementMovementAnimation.RotateClockwise: _MoveRotate(false); break;
                case ElementMovementAnimation.RotateAntiClockwise: _MoveRotate(true);break;
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
        EmitSignal(SignalName.ElementTransformed);
        SetElement(element);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void SetElement(ElementType element)
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

        if (m_data.m_ElementBehaviorNode != null && m_data.m_ElementBehaviorNode != null)
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
    public void MoveElement(Vector2 newPos, ElementMovementAnimation animation)
    {
        m_IsMoving = true;
        m_StartPos = Position;
        m_TargetPos = newPos;

        m_Animation = animation;
        if (animation == ElementMovementAnimation.RotateClockwise || animation == ElementMovementAnimation.RotateAntiClockwise)
        {
            Vector2 startRadiusVector = GlobalPosition - m_Board.m_MiddleOfTheBoard;
            m_StartSquareRadius = startRadiusVector.LengthSquared();
            m_StartAngle = startRadiusVector.Angle();

            Vector2 endRadiusVector = m_TargetPos - Position + GlobalPosition - m_Board.m_MiddleOfTheBoard;
            m_TargetSquareRadius = endRadiusVector.LengthSquared();
            m_TargetAngle = endRadiusVector.Angle();
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
    public void RemovedFromBoard()
    {
        // Animation, VFX, this kind of thing. For now just remove it.
        if (m_Behavior != null)
        {
            m_Behavior.ClearBehavior();
            m_Behavior.QueueFree();
        }
        QueueFree();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void EndMove()
    {
        Position = m_TargetPos;
        m_IsMoving = false;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _MoveDefault()
    {
        Position = m_StartPos.Lerp(m_TargetPos, m_Board.m_MovingElementLerp);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _MoveRotate(bool anticlockwise)
    {
        float currentSquareRadius = Mathf.Lerp(m_StartSquareRadius, m_TargetSquareRadius, m_Board.m_MovingElementLerp);
        float trueStartAngle = m_StartAngle;
        float trueTargetAngle = m_TargetAngle;
        if (anticlockwise)
        {
            while (trueStartAngle < trueTargetAngle)
            {
                trueTargetAngle -= Mathf.DegToRad(360.0f);
            }
        }
        else
        {
            while (trueStartAngle > trueTargetAngle)
            {
                trueTargetAngle += Mathf.DegToRad(360.0f);
            }
        }

        float currentAngle = Mathf.Lerp(trueStartAngle, trueTargetAngle, m_Board.m_MovingElementLerp);
        Vector2 newVectorRadius = Vector2.FromAngle(currentAngle) * Mathf.Sqrt(currentSquareRadius);
        GlobalPosition = m_Board.m_MiddleOfTheBoard + newVectorRadius;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _MoveFadeOutFadeIn()
    {
        Color color = Modulate;
        if (m_Board.m_MovingElementLerp < 0.5f)
        {
            color.A = Mathf.Lerp(1, 0, m_Board.m_MovingElementLerp * 2.0f);
        }
        else
        {
            Position = m_TargetPos;
            color.A = Mathf.Lerp(0, 1, (m_Board.m_MovingElementLerp * 2.0f) - 1.0f);
        }
        Modulate = color;
    }
}
