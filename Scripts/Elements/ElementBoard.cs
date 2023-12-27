using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class ElementBoard : Node2D
{
    // STATIC PART ----------------------------
    static ElementBoard g_Board;

    static public ElementBoard GetBoard()
    {
        return g_Board;
    }
    // STATIC PART ----------------------------

    [Export] public BoardHelper m_Helper;

    [Export] public int m_Size = 10;

    private Element[,] m_GameBoard;

    private StateMachine m_StateMachine = new StateMachine();

    private List<ElementsMatch> m_CurrentMatch = new List<ElementsMatch>();

    public float m_MovingElementLerp = 1.0f;

    public bool m_DrawCardNextPhase = false;
    public bool m_GainManaNextPhase = false;
    public bool m_DestroyDontTrigger = false;

    public Vector2 m_MiddleOfTheBoard;
    public Vector2 m_MiddleOfTheBoardCoordinate;

    public int m_MatchCount = 4;

    private float m_MatchMultiplier;

    // -----------------------------------------------------------------
    // Called when the node enters the scene tree for the first time.
    // -----------------------------------------------------------------
    public override void _Ready()
    {
        g_Board = this;
        m_GameBoard = new Element[m_Size, m_Size];
        m_StateMachine.SetCurrentStateFunction(State_WaitingForInput);

        
        m_MiddleOfTheBoardCoordinate = new Vector2((float)(m_Size - 1) / 2.0f, (float)(m_Size - 1) / 2.0f);
        m_MiddleOfTheBoard = GlobalPosition + m_MiddleOfTheBoardCoordinate * Element.ElementSize + Vector2.One * Element.ElementHalfSize;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Process(double delta)
    {
        m_StateMachine.UpdateStateMachine();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void BoardInputEvent(Node viewport, InputEvent generatedEvent, int shapeIdx)
    {
        m_Helper.ClearHint();
        if (m_StateMachine.IsCurrentState(State_WaitingForInput) == false || BattleManager.GetManager().IsPlayerTurn() == false)
        {
            return;
        }

        InputEventMouse mouseEvent = (InputEventMouse)generatedEvent;
        int buttonLeftPressed = ((int)mouseEvent.ButtonMask) & ((int)MouseButton.Left);
        Card currentCard = BattleManager.GetManager().m_CurrentHeldCard;

        if (currentCard != null)
        {
            Vector2 pos = mouseEvent.Position - Position;

            Vector2I index = new Vector2I((int)pos.X, (int)pos.Y) / (int)Element.ElementSize;
            Vector2 intraElementSelectPos = (pos / Element.ElementSize) - index - new Vector2(0.5f, 0.5f);
            if (buttonLeftPressed != 0)
            {
                currentCard.ApplyCardToBoard(this, index, intraElementSelectPos);
            }
            else
            {
                currentCard.UpdatePreview(this, index, intraElementSelectPos);
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Swap(List<Vector2I> elementList)
    {
        foreach (Vector2I coord in elementList)
        {
            if (CheckCoordinate(coord) == false)
            {
                return;
            }
        }

        for (int i = 0; i * 2 < elementList.Count; i++)
        {
            Vector2I coord1 = elementList[i];
            Vector2I coord2 = elementList[elementList.Count - i - 1];

            Element elem1 = m_GameBoard[coord1.X, coord1.Y];
            Element elem2 = m_GameBoard[coord2.X, coord2.Y];

            m_GameBoard[coord1.X, coord1.Y] = elem2;
            m_GameBoard[coord2.X, coord2.Y] = elem1;

            elem1.MoveElement(elem2.Position, ElementMovementAnimation.Default);
            elem2.MoveElement(elem1.Position, ElementMovementAnimation.Default);
        }
    }
    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RotateTheBoard(bool anticlockwise)
    {
        Element[,] TempBoard = new Element[m_Size, m_Size];
        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el != null)
                {
                    Vector2I newCoordinate = ComputeCoordinateRotation(new Vector2I(x, y), anticlockwise);
                    TempBoard[newCoordinate.X, newCoordinate.Y] = el;
                    el.MoveElement(
                        new Vector2(newCoordinate.X * Element.ElementSize + Element.ElementHalfSize, 
                                    newCoordinate.Y * Element.ElementSize + Element.ElementHalfSize),
                    el.m_Type == ElementType.Void ? ElementMovementAnimation.FadeOutFadeIn :
                    anticlockwise ? ElementMovementAnimation.RotateAntiClockwise : ElementMovementAnimation.RotateClockwise);
                    m_GameBoard[x,y] = null;
                }
            }
        }

        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                m_GameBoard[x, y] = TempBoard[x, y];
            }
        }
    }

    
    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public Vector2I ComputeCoordinateRotation(Vector2I coordinate, bool anticlockwise)
    {
        Vector2I newCoordinate = new Vector2I(coordinate.X, coordinate.Y);

        if (anticlockwise)
        {
            newCoordinate.X = coordinate.Y;
            newCoordinate.Y = m_Size - 1 - coordinate.X;
        }
        else
        {
            newCoordinate.X = m_Size - 1 - coordinate.Y;
            newCoordinate.Y = coordinate.X;
        }

        return newCoordinate;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckCoordinate(Vector2I coord)
    {
        return coord.X >= 0 && coord.X < m_Size 
            && coord.Y >= 0 && coord.Y < m_Size
            && m_GameBoard[coord.X, coord.Y] != null
            && m_GameBoard[coord.X, coord.Y].m_Type != ElementType.Void;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckBoardState()
    {
        bool needRecheck = false;

        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el != null)
                {
                    needRecheck |= _CheckLineRec(new Vector2I(x, y), Vector2I.Right, m_MatchCount, el.m_Type, true) != null;
                    needRecheck |= _CheckLineRec(new Vector2I(x, y), Vector2I.Down, m_MatchCount, el.m_Type, true) != null;
                }
            }
        }

        return needRecheck;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    private ElementsMatch _CheckLineRec(Vector2I coordinate, Vector2I directionCheck, int lineMax, ElementType type, bool markForMatch)
    {
        if (coordinate.X >= m_Size || coordinate.Y >= m_Size || coordinate.X < 0 || coordinate.Y < 0)
        {
            return null;
        }

        Element el = m_GameBoard[coordinate.X, coordinate.Y];
        if (el == null || el.CanMatch() == false || el.m_Type != type)
        {
            return null;
        }

        ElementsMatch foundMatch = el.GetMatch(directionCheck);
        if (foundMatch == null)
        {
            // If this element isn't already part of a match in a specific direction :
            Vector2I nextCoord = coordinate + directionCheck;
            foundMatch = _CheckLineRec(nextCoord, directionCheck, lineMax - 1, type, markForMatch);

            if (lineMax <= 1 && foundMatch == null)
            {
                foundMatch = new ElementsMatch();
                foundMatch.m_Type = type;
                foundMatch.m_matchDirection = directionCheck;

                if (markForMatch)
                {
                    m_CurrentMatch.Add(foundMatch);
                }
            }

            if (foundMatch != null && markForMatch)
            {
                el.SetMatch(directionCheck, foundMatch);
                el.m_ToDelete = true;
            }
        }

        return foundMatch;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void DeleteMarkedElement()
    {
        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el != null && el.m_ToDelete)
                {
                    m_GameBoard[x, y] = null;
                    el.RemovedFromBoard();
                }
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void EndElementMove()
    {
        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el != null)
                {
                    el.EndMove();
                }
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void TriggerDetectedMatches()
    {
        // 
        m_MatchMultiplier += (m_CurrentMatch.Count - 1) * 0.1f;
        foreach (ElementsMatch match in m_CurrentMatch)
        {
            match.OnMatch(Mathf.Min(m_MatchMultiplier, 2.5f));
        }

        m_MatchMultiplier += 0.1f;
        m_CurrentMatch.Clear();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool GravityCheck()
    {
        bool returnVal = false;
        for (int x = m_Size - 1; x >= 0; x--)
        {
            for (int y = m_Size - 1; y >= 0; y--)
            {
                if (m_GameBoard[x, y] == null && y != 0)
                {
                    int ySearch = y;
                    Element el;
                    do
                    {
                        ySearch--;
                        el = m_GameBoard[x, ySearch];
                    } while ((el == null || el.m_Type == ElementType.Void) && ySearch > 0);

                    if (el != null && el.m_Type != ElementType.Void)
                    {
                        returnVal = true;
                        m_GameBoard[x, y] = m_GameBoard[x, ySearch];
                        m_GameBoard[x, ySearch] = null;
                        // TODO : This is icky
                        el.MoveElement(new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize), ElementMovementAnimation.Default);
                    }
                }
            }
        }
        return returnVal;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void InitBoard(string boardLayoutRes)
    {
        Godot.FileAccess f = Godot.FileAccess.Open(boardLayoutRes, Godot.FileAccess.ModeFlags.Read);
        string boardLayoutText = f.GetAsText();

        GD.Print(boardLayoutRes);

        int x = 0;
        int y = 0;
        foreach(char c in boardLayoutText)
        {
            if (c == '\n')
            {
                x = 0;
                y++;
            }
            else if (c == '0')
            {
                Element newVoid = Element.CreateElementFromType(ElementType.Void);
                AddChild(newVoid);
                // TODO : This is icky
                newVoid.Init(
                    this, 
                    new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize), 
                    new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize));
                m_GameBoard[x, y] = newVoid;
                x++;
            }
            else if (c == '1')
            {
                x++;
            }
        }

        m_StateMachine.SetCurrentStateFunction(State_FillBoard);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void ClearBoard()
    {
        for (int x = m_Size - 1; x >= 0; x--)
        {
            for (int y = m_Size - 1; y >= 0; y--)
            {
                if (m_GameBoard[x, y] != null)
                {
                    m_GameBoard[x, y].QueueFree();
                    m_GameBoard[x, y] = null;
                }
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void AddManualMatch(List<Vector2I> elementList)
    {
        Debug.Assert(elementList.Count > 0, "Tried to make a manual match with empty element");
        ElementsMatch newMatch = new ElementsMatch();
        newMatch.m_Type = GetElement(elementList[0]).m_Type;
        newMatch.m_matchDirection = Vector2I.Zero;
        foreach(Vector2I coord in elementList)
        {
            Element el = GetElement(coord);
            
            el.SetMatch(Vector2I.Zero, newMatch);
            el.m_ToDelete = true;
        }

        m_CurrentMatch.Add(newMatch);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void FillBoard()
    {
        for (int x = m_Size - 1; x >= 0; x--)
        {
            for (int y = m_Size - 1; y >= 0; y--)
            {
                if (m_GameBoard[x, y] == null)
                {
                    Element newElement = null;
                    ElementType newType;
                    bool needReroll;
                    do
                    {
                        needReroll = false;
                        newType = Element.GetRandomType();

                        for(int iOffset = 0; iOffset < m_MatchCount && needReroll == false; iOffset++)
                        {
                            bool currentCheckVertical = true;
                            bool currentCheckHorizontal = true;
                            if (iOffset > 0)
                            {
                                currentCheckHorizontal = _CheckLineRec(new Vector2I(x - 1, y), Vector2I.Left, iOffset, newType, false) != null;
                                currentCheckVertical = _CheckLineRec(new Vector2I(x, y - 1), Vector2I.Up, iOffset, newType, false) != null;
                            }

                            if (iOffset < m_MatchCount - 1)
                            {
                                currentCheckHorizontal = currentCheckHorizontal && 
                                    _CheckLineRec(new Vector2I(x + 1, y), Vector2I.Right, m_MatchCount - iOffset - 1, newType, false) != null;
                                currentCheckVertical = currentCheckVertical && 
                                    _CheckLineRec(new Vector2I(x, y + 1), Vector2I.Down, m_MatchCount - iOffset - 1, newType, false) != null;
                            }

                            needReroll |= currentCheckHorizontal || currentCheckVertical;
                        }

                        // This is absolutely shit. Need to be a little bit smarter here.
//                      needReroll = needReroll || _CheckLineRec(new Vector2I(x + 1, y), Vector2I.Right, 3, newType, false) != null;
//                      needReroll = needReroll || _CheckLineRec(new Vector2I(x, y + 1), Vector2I.Down, 3, newType, false) != null;
//                      needReroll = needReroll || _CheckLineRec(new Vector2I(x + 1, y), Vector2I.Right, 2, newType, false) != null
//                                              && _CheckLineRec(new Vector2I(x - 1, y), Vector2I.Left, 1, newType, false) != null;
//                      needReroll = needReroll || _CheckLineRec(new Vector2I(x + 1, y), Vector2I.Right, 1, newType, false) != null
//                                              && _CheckLineRec(new Vector2I(x - 1, y), Vector2I.Left, 2, newType, false) != null;
//                      needReroll = needReroll || _CheckLineRec(new Vector2I(x - 1, y), Vector2I.Left, 3, newType, false) != null;
                    } while (needReroll);
                    
                    newElement = Element.CreateElementFromType(newType);
                    AddChild(newElement);
                    // TODO : This is icky
                    newElement.Init(
                        this, 
                        new Vector2(x * Element.ElementSize + Element.ElementHalfSize, -Element.ElementHalfSize), 
                        new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize));
                    m_GameBoard[x, y] = newElement;
                }
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CanFillTheBoard()
    {
        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el == null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public Element GetElement(Vector2I coordinate)
    {
        if (coordinate.X >= m_Size || coordinate.Y >= m_Size || coordinate.X < 0 || coordinate.Y < 0)
        {
            return null;
        }

        return m_GameBoard[coordinate.X, coordinate.Y];
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void SetStateToDestroyElement()
    {
        m_StateMachine.SetCurrentStateFunction(State_DestroyElements);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void SetStateToMoveElement()
    {
        m_StateMachine.SetCurrentStateFunction(State_MovingElements);
    }


    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void MarkElementToDelete(Vector2I coordinate)
    {
        if (coordinate.X >= m_Size || coordinate.Y >= m_Size || coordinate.X < 0 || coordinate.Y < 0)
        {
            return;
        }

        m_GameBoard[coordinate.X, coordinate.Y].m_ToDelete = true;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public StateFunc State_WaitingForInput(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter:
                {
                    m_MatchMultiplier = 1.0f;
                    break;
                }
            case StateFunctionCall.Update:
                {
                    // N/A
                    break;
                }
            case StateFunctionCall.Exit:
                {
                    // N/A
                    break;
                }
        }
        return null;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public StateFunc State_MovingElements(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter:
                {
                    m_MovingElementLerp = 0.0f;
                    break;
                }
            case StateFunctionCall.Update:
                {
                    m_MovingElementLerp += TimeManager.GetDeltaTime() * 2.0f;
                    if (m_MovingElementLerp > 1.0f)
                    {
                        m_MovingElementLerp = 1.0f;
                        if (CheckBoardState())
                        {
                            return State_DestroyElements;
                        }
                        else if (BattleManager.GetManager().CheckEndFight())
                        {
                            ClearBoard();
                            BattleManager.GetManager().EndFight();
                        }
                        else if (CanFillTheBoard())
                        {
                            return State_FillBoard;
                        }
                        return State_WaitingForInput;

                    }
                    break;
                }
            case StateFunctionCall.Exit:
                {
                    EndElementMove();
                    break;
                }
        }
        return null;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public StateFunc State_DestroyElements(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter:
                {
                    TriggerDetectedMatches();
                    DeleteMarkedElement();
                    break;
                }
            case StateFunctionCall.Update:
                {
                    m_MovingElementLerp += TimeManager.GetDeltaTime() * 2.0f;
                    if (m_MovingElementLerp > 1.0f)
                    {
                        if (GravityCheck())
                        {
                            return State_MovingElements;
                        }
                        else
                        {
                            return State_FillBoard;
                        }

                    }
                    break;
                }
            case StateFunctionCall.Exit:
                {
                    m_DestroyDontTrigger = false;
                    break;
                }
        }
        return null;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public StateFunc State_FillBoard(StateFunctionCall a_Call)
    {
        switch (a_Call)
        {
            case StateFunctionCall.Enter:
                {
                    FillBoard();
                    break;
                }
            case StateFunctionCall.Update:
                {
                    // This state purely exist to reset the MovingElement State.
                    return State_MovingElements;
                }
            case StateFunctionCall.Exit:
                {
                    // N/A
                    break;
                }
        }
        return null;
    }
}
