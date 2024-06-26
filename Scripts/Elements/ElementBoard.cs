using Godot;
using System.Collections.Generic;

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

    private List<ElementsMatch> m_CurrentMatch = new List<ElementsMatch>();

    public float m_MovingElementLerp = 0.0f;

    public bool m_DrawCardNextPhase = false;
    public bool m_GainManaNextPhase = false;
    public bool m_DestroyDontTrigger = false;

    public Vector2 m_MiddleOfTheBoard;
    public Vector2 m_MiddleOfTheBoardCoordinate;

    public int m_MatchCount = 4;
    public bool m_PuzzleMode = false;

    private float m_MatchMultiplier;
    private float m_BaseMatchMultiplier = 1.0f;
    private float m_IncrementMatchMultiplier = 1.0f;
    private float m_MaxMatchMultiplier = 2.5f;

    private bool m_MovementAlreadyRequestedThisFrame = false;

    private int m_BlockingCheckUntilEndOfTurn;

    // -----------------------------------------------------------------
    // Called when the node enters the scene tree for the first time.
    // -----------------------------------------------------------------
    public override void _Ready()
    {
        g_Board = this;
        m_GameBoard = new Element[m_Size, m_Size];

        
        m_MiddleOfTheBoardCoordinate = new Vector2((float)(m_Size - 1) / 2.0f, (float)(m_Size - 1) / 2.0f);
        m_MiddleOfTheBoard = GlobalPosition + m_MiddleOfTheBoardCoordinate * Element.ElementSize + Vector2.One * Element.ElementHalfSize;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public override void _Process(double delta)
    {
        base._Process(delta);
        m_MovementAlreadyRequestedThisFrame = false;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Swap(List<Vector2I> elementList, ElementMovementAnimation anim = ElementMovementAnimation.Default)
    {
        // No check anymore, if we are here, that mean we want to swap no matter what.
//        foreach (Vector2I coord in elementList)
//        {
//            if (CheckCoordinateForMovement(coord) == false)
//            {
//                return;
//            }
//        }

        for (int i = 0; i * 2 < elementList.Count; i++)
        {
            Vector2I coord1 = elementList[i];
            Vector2I coord2 = elementList[elementList.Count - i - 1];

            Element elem1 = m_GameBoard[coord1.X, coord1.Y];
            Element elem2 = m_GameBoard[coord2.X, coord2.Y];

            m_GameBoard[coord1.X, coord1.Y] = elem2;
            m_GameBoard[coord2.X, coord2.Y] = elem1;

            elem1.MoveElement(elem2.Position, anim);
            elem2.MoveElement(elem1.Position, anim);
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void OffsetElements(List<Vector2I> elementList, int offset, ElementMovementAnimation anim = ElementMovementAnimation.Default)
    {
        List<Element> elementToMove = new List<Element>();

        for(int i = 0; i < elementList.Count; i++)
        {
            Vector2I coordinate = elementList[i];
            elementToMove.Add(m_GameBoard[coordinate.X, coordinate.Y]);
        }

        for(int i = 0; i < elementToMove.Count; i++)
        {
            int newCoordinateIndex = (i + elementToMove.Count + offset) % elementToMove.Count;
            Vector2I newCoordinate = elementList[newCoordinateIndex];
            Element el = elementToMove[i];
            
            m_GameBoard[newCoordinate.X, newCoordinate.Y] = el;

            el.MoveElement((Vector2)newCoordinate * Element.ElementSize + Vector2.One * Element.ElementHalfSize, anim);
        }

    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void ResetElementsPosition()
    {
        for (int x = m_Size - 1; x >= 0; x--)
        {
            for (int y = m_Size - 1; y >= 0; y--)
            {
                m_GameBoard[x, y].Position = new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize);
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void PreviewLineDrag(int yLine, float offset)
    {
        List<int> coordinateToOffset = new List<int>();
        for (int x = 0; x < m_Size; x++)
        {
            if (m_GameBoard[x, yLine].CanBeMoved())
            {
                coordinateToOffset.Add(x);
            }
        }

        float roundedOffset = Mathf.Round(offset);
        float decimalOffset = offset - roundedOffset;    
        for (int i = 0; i < coordinateToOffset.Count; i++)
        {
            int oldX = coordinateToOffset[i];
            int newIndex = i + (int) roundedOffset;
            while (newIndex < 0) { newIndex += coordinateToOffset.Count; }
            while (newIndex >= coordinateToOffset.Count) { newIndex -= coordinateToOffset.Count; }
            int newX = coordinateToOffset[newIndex];

            m_GameBoard[oldX, yLine].Position = new Vector2((newX + decimalOffset) * Element.ElementSize + Element.ElementHalfSize + decimalOffset, m_GameBoard[oldX, yLine].Position.Y);
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void PreviewColumnDrag(int xLine, float offset)
    {
        List<int> coordinateToOffset = new List<int>();
        for (int y = 0; y < m_Size; y++)
        {
            if (m_GameBoard[xLine, y].CanBeMoved())
            {
                coordinateToOffset.Add(y);
            }
        }

        float roundedOffset = Mathf.Round(offset);
        float decimalOffset = offset - roundedOffset;  
        for (int i = 0; i < coordinateToOffset.Count; i++)
        {
            int oldY = coordinateToOffset[i];
            int newIndex = i + (int) roundedOffset;
            while (newIndex < 0) { newIndex += coordinateToOffset.Count; }
            while (newIndex >= coordinateToOffset.Count) { newIndex -= coordinateToOffset.Count; }
            int newY = coordinateToOffset[newIndex];

            m_GameBoard[xLine, oldY].Position = new Vector2(m_GameBoard[xLine, oldY].Position.X, (newY + decimalOffset) * Element.ElementSize + Element.ElementHalfSize + decimalOffset);
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
    public bool CheckCoordinateForTransform(Vector2I coord)
    {
        return CheckCoordinate(coord) && m_GameBoard[coord.X, coord.Y].CanBeTransformed();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckCoordinateForMovement(Vector2I coord)
    {
        return CheckCoordinate(coord) && m_GameBoard[coord.X, coord.Y].CanBeMoved();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckCoordinateForDestroy(Vector2I coord)
    {
        return CheckCoordinate(coord) && m_GameBoard[coord.X, coord.Y].CanBeDestroyed();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckCoordinateForMatch(Vector2I coord)
    {
        return CheckCoordinate(coord) && m_GameBoard[coord.X, coord.Y].CanMatch();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CanCheckBoard()
    {
        return m_BlockingCheckUntilEndOfTurn == 0 || BattleManager.GetManager().IsEndOfTurn();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RequestBlockingCheckUntilEndOfTurn()
    {
        m_BlockingCheckUntilEndOfTurn++;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RequestUnblockingCheckUntilEndOfTurn()
    {
        m_BlockingCheckUntilEndOfTurn--;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void ForceCheckBoardForMatch()
    {
        if (_CheckBoardState())
        {
            RequestToDestroyElement();
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    private bool _CheckBoardState()
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
    public Vector2I GetElementCoordinate(Element elToGet)
    {
        for (int x = 0; x < m_Size; x++)
        {
            for (int y = 0; y < m_Size; y++)
            {
                Element el = m_GameBoard[x, y];
                if (el == elToGet)
                {
                    return new Vector2I(x, y);
                }
            }
        }
        return new Vector2I(m_Size, m_Size);
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
                    el.DestroyElement();
                }
            }
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RemoveElementFromBoard(Element el)
    {
        Vector2I coordinate = GetElementCoordinate(el);
        m_GameBoard[coordinate.X, coordinate.Y] = null;
    }
    

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void TriggerDetectedMatches()
    {
        m_MatchMultiplier += (m_CurrentMatch.Count - 1) * m_IncrementMatchMultiplier;
        foreach (ElementsMatch match in m_CurrentMatch)
        {
            match.OnMatch(Mathf.Min(m_MatchMultiplier, m_MaxMatchMultiplier));
        }
        EventManager.GetManager().EmitSignal(EventManager.SignalName.OnMatches);

        m_MatchMultiplier += m_IncrementMatchMultiplier;
        m_CurrentMatch.Clear();
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void ResetMultiplier()
    {
        m_MatchMultiplier = m_BaseMatchMultiplier;
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
                    } while ((el == null || el.CanFall() == false) && ySearch > 0);

                    if (el != null && el.CanFall())
                    {
                        returnVal = true;
                        m_GameBoard[x, y] = m_GameBoard[x, ySearch];
                        m_GameBoard[x, ySearch] = null;
                        // TODO : This is icky
                        el.MoveElement(new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize), ElementMovementAnimation.Gravity);
                    }
                }
            }
        }
        return returnVal;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool CheckAllElementAreIdle()
    {
        for (int x = m_Size - 1; x >= 0; x--)
        {
            for (int y = m_Size - 1; y >= 0; y--)
            {
                Element el = m_GameBoard[x, y];
                if (el != null && el.IsIdle() == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    private void _InitElement(int x, int y, ElementType type)
    {
        Element newVoid = Element.CreateElementFromType(type);
        AddChild(newVoid);
        // TODO : This is icky
        newVoid.Init(
            this, 
            new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize), 
            new Vector2(x * Element.ElementSize + Element.ElementHalfSize, y * Element.ElementSize + Element.ElementHalfSize));
        m_GameBoard[x, y] = newVoid;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void InitBoard(string boardLayoutRes)
    {
        Godot.FileAccess f = Godot.FileAccess.Open(boardLayoutRes, Godot.FileAccess.ModeFlags.Read);
        string boardLayoutText = f.GetAsText();

        int x = 0;
        int y = 0;
        foreach(char c in boardLayoutText)
        {
            switch (c)
            {
                case '\n': x = 0; y++;                                       break;
                case 'X':                                               x++; break;
                case '>': _InitElement(x, y, ElementType.Rotator);      x++; break;
                case ' ': _InitElement(x, y, ElementType.Void);         x++; break;
                case '1': _InitElement(x, y, ElementType.Element1);     x++; break;
                case '2': _InitElement(x, y, ElementType.Element2);     x++; break;
                case '3': _InitElement(x, y, ElementType.Element3);     x++; break;
                case '4': _InitElement(x, y, ElementType.Element4);     x++; break;
                case 'R': _InitElement(x, y, ElementType.RockElement);  x++; break;
            }
        }

        RequestToFillBoard();
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
                    ElementType newType;
                    ElementDataBase elementDb = GameManager.GetManager().m_ElementDatabase;
                    do
                    {
                        newType = elementDb.GetRandomElementData();
                    } while (CheckIfElementCreateMatchAtCoordinate(new Vector2I(x,y), newType));
                    
                    Element newElement = Element.CreateElementFromType(newType);
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
    public bool CheckIfElementCreateMatchAtCoordinate(Vector2I coord, ElementType type)
    {
        bool willCreateMatch = false;
        for(int iOffset = 0; iOffset < m_MatchCount && willCreateMatch == false; iOffset++)
        {
            bool currentCheckVertical = true;
            bool currentCheckHorizontal = true;
            if (iOffset > 0)
            {
                currentCheckHorizontal = _CheckLineRec(new Vector2I(coord.X - 1, coord.Y), Vector2I.Left, iOffset, type, false) != null;
                currentCheckVertical = _CheckLineRec(new Vector2I(coord.X, coord.Y - 1), Vector2I.Up, iOffset, type, false) != null;
            }

            if (iOffset < m_MatchCount - 1)
            {
                currentCheckHorizontal = currentCheckHorizontal && 
                    _CheckLineRec(new Vector2I(coord.X + 1, coord.Y), Vector2I.Right, m_MatchCount - iOffset - 1, type, false) != null;
                currentCheckVertical = currentCheckVertical && 
                    _CheckLineRec(new Vector2I(coord.X, coord.Y + 1), Vector2I.Down, m_MatchCount - iOffset - 1, type, false) != null;
            }

            willCreateMatch |= currentCheckHorizontal || currentCheckVertical;
        }

        return willCreateMatch;
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
    public Element GetElement(int x, int y)
    {
        return GetElement(new Vector2I(x,y));
    }
    

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void ComputeChunkElement(ref List<Vector2I> listToCompute, Vector2I selectedElement)
    {
        listToCompute.Clear();
        Element element = GetElement(selectedElement);
        if (element == null)
        {
            return;
        }

        _ComputeChunkElementRec(ref listToCompute, selectedElement, element.m_Type);
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _ComputeChunkElementRec(ref List<Vector2I> listToCompute, Vector2I selectedElement, ElementType elementType)
    {
        Element element = GetElement(selectedElement);
        if (element == null || element.m_Type != elementType || listToCompute.Contains(selectedElement))
        {
            return;
        }

        listToCompute.Add(selectedElement);
        _ComputeChunkElementRec(ref listToCompute, selectedElement + new Vector2I(0,1), elementType);
        _ComputeChunkElementRec(ref listToCompute, selectedElement + new Vector2I(1,0), elementType);
        _ComputeChunkElementRec(ref listToCompute, selectedElement + new Vector2I(-1,0), elementType);
        _ComputeChunkElementRec(ref listToCompute, selectedElement + new Vector2I(0,-1), elementType);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RequestToDestroyElement()
    {
        BattleManager.GetManager().AddToStateQueue(Queue_DestroyElements, 10);
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RequestToMoveElement()
    {
        if (m_MovementAlreadyRequestedThisFrame == false)
        {
            BattleManager.GetManager().AddToStateQueue(Queue_MovingElements, 10);
            m_MovementAlreadyRequestedThisFrame = true;
        }
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void RequestToFillBoard()
    {
        BattleManager.GetManager().AddToStateQueue(Queue_FillBoard, 10);
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
    public bool Queue_MovingElements(QueueFuncCall a_Call)
    {
        switch (a_Call)
        {
            case QueueFuncCall.Activation:
                {
                    m_MovingElementLerp = 0.0f;
                    break;
                }
            case QueueFuncCall.Update:
                {
                    m_MovingElementLerp += TimeManager.GetDeltaTime() * 2.0f;
                    if (m_MovingElementLerp > 1.0f)
                    {
                        m_MovingElementLerp = 1.0f;
                        if (CheckAllElementAreIdle())
                        {
                            if (CanCheckBoard() && _CheckBoardState())
                            {
                                RequestToDestroyElement();
                            }
                            else if (BattleManager.GetManager().CheckEndFight())
                            {
                                ClearBoard();
                                BattleManager.GetManager().EndFight();
                            }
                            else if (CanFillTheBoard() && m_PuzzleMode == false)
                            {
                                RequestToFillBoard();
                            }
                            m_MovingElementLerp = 0.0f;
                            return true;
                        }

                    }
                    break;
                }
        }
        return false;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool Queue_DestroyElements(QueueFuncCall a_Call)
    {
        switch (a_Call)
        {
            case QueueFuncCall.Activation:
                {
                    TriggerDetectedMatches();
                    DeleteMarkedElement();
                    break;
                }
            case QueueFuncCall.Update:
                {
                    if (CheckAllElementAreIdle())
                    {
                        if (GravityCheck())
                        {
                            RequestToMoveElement();
                        }
                        else
                        {
                            RequestToFillBoard();
                        }
                        return true;
                    }
                    break;
                }
        }
        return false;
    }

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public bool Queue_FillBoard(QueueFuncCall a_Call)
    {
        switch (a_Call)
        {
            case QueueFuncCall.Activation:
                {
                    if (m_PuzzleMode == false)
                    {
                        FillBoard();
                    }
                    break;
                }
            case QueueFuncCall.Update:
                {
                    // This state purely exist to reset the MovingElement State.
                    if (m_PuzzleMode == false)
                    {
                        RequestToMoveElement();
                    }
                    else
                    {
                        if (BattleManager.GetManager().CheckEndFight())
                        {
                            ClearBoard();
                            BattleManager.GetManager().EndFight();
                        }
                    }
                    return true;
                }
                
        }
        return false;
    }
}
