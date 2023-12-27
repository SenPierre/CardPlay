using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class BoardHelper : Node2D
{
    [Export] public PackedScene m_SelectionObjectScene;
    [Export] public ElementBoard m_Board;

    Stack<NinePatchRect> m_DisplayedHints = new Stack<NinePatchRect>();
    Stack<NinePatchRect> m_HintReserve = new Stack<NinePatchRect>();

    // Okay, 

    // --------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------
    public void ClearHint()
    {
        while(m_DisplayedHints.Count > 0)
        {
            NinePatchRect item = m_DisplayedHints.Pop();
            m_HintReserve.Push(item);
            item.Visible = false;
        }
    }
    
    // --------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------
    public void AddHint(Vector2I min, Vector2I max, Color color)
    {
        NinePatchRect newPatch = GetAnHint();
        m_DisplayedHints.Push(newPatch);
        newPatch.Position = new Vector2(min.X, min.Y) * 80.0f;
        Vector2I intSize = max - min + new Vector2I(1,1);
        newPatch.Size = new Vector2(intSize.X, intSize.Y) * 80.0f;
        newPatch.Modulate = color;
        newPatch.Visible = true;
    }

    // --------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------
    private NinePatchRect GetAnHint()
    {
        if (m_HintReserve.Count > 0)
        {
            return m_HintReserve.Pop();
        }
        else
        {
            NinePatchRect newPatch = m_SelectionObjectScene.Instantiate<NinePatchRect>();
            AddChild(newPatch);
            return newPatch;
        }
    }
}
