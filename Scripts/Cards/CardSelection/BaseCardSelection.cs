using Godot;
using System;
using System.Collections.Generic;

public enum SelectionKind {
    ForMovement,
    ForTransform,
    ForDestroy,
    ForForcedMatch,
    ForGenericPurpose
}

public enum SelectionStatus {
    SelectionIncomplete,
    SelectionComplete,
    SelectionInvalid,
}

//###############################################################################
//
//###############################################################################
[GlobalClass]
public abstract partial class BaseCardSelection : Resource
{
    [Export] public SelectionKind m_SelectionKind = SelectionKind.ForGenericPurpose;
    public List<Vector2I> m_ElementList = new List<Vector2I>();
    public SelectionStatus m_SelectionStatus = SelectionStatus.SelectionIncomplete;
    
    public abstract void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset);
    public abstract void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset);
    public void Reset()
    {
        m_ElementList.Clear();
        m_SelectionStatus = SelectionStatus.SelectionIncomplete;
    }

    protected bool _CheckCoordinate(ElementBoard gameBoard, Vector2I coordinateToCheck)
    {
        switch (m_SelectionKind)
        {
            case SelectionKind.ForDestroy: return gameBoard.CheckCoordinateForDestroy(coordinateToCheck);
            case SelectionKind.ForTransform: return gameBoard.CheckCoordinateForTransform(coordinateToCheck);
            case SelectionKind.ForMovement: return gameBoard.CheckCoordinateForMovement(coordinateToCheck);
            case SelectionKind.ForForcedMatch: return gameBoard.CheckCoordinateForMatch(coordinateToCheck);
            case SelectionKind.ForGenericPurpose: return gameBoard.CheckCoordinate(coordinateToCheck);
        }
        return false;
    }
}