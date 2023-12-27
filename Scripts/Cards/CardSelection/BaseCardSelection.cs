using Godot;
using System;
using System.Collections.Generic;

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
    public List<Vector2I> m_ElementList = new List<Vector2I>();
    public SelectionStatus m_SelectionStatus = SelectionStatus.SelectionIncomplete;
    
    public abstract void ApplySelectionPreview(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset);
    public abstract void Select(ElementBoard gameBoard, Vector2I selectedElement, Vector2 clickCenterOffset);
    public void Reset()
    {
        m_ElementList.Clear();
        m_SelectionStatus = SelectionStatus.SelectionIncomplete;
    }
}