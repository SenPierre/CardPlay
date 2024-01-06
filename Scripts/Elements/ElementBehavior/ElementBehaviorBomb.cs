using Godot;

public partial class ElementBehaviorBomb : BaseElementBehavior
{
    [Export] public int m_CountdownBase = 2;
    [Export] public int m_radius = 1;
    [Export] public Label m_Label;

    private int m_Countdown = 3;

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void InitBehavior(Element el) 
    {
        base.InitBehavior(el);
        m_Countdown = m_CountdownBase;
        _UpdateLabel();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void ClearBehavior() 
    {

    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void OnTurnEnd()
    {
        m_Countdown--;
        if (m_Countdown == 0)
        {
            _KABOOM(); 
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _KABOOM()
    {
        ElementBoard board = ElementBoard.GetBoard();
        Vector2I coordinate = board.GetElementCoordinate(m_element);
        for(int x = -m_radius; x <= m_radius; x++)
        {
            for(int y = -m_radius; y <= m_radius; y++)
            {
                Element el = board.GetElement(coordinate + new Vector2I(x, y));
                if (el != null && el.CanBeDestroyed())
                {
                    el.m_ToDelete = true;
                }
            }
        }
        
        _UpdateLabel();
        board.SetStateToDestroyElement();
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    private void _UpdateLabel()
    {
        if (m_Countdown <= 0)
        {
            m_Label.Visible = false;
        }
        else
        {
            m_Label.Visible = true;
            m_Label.Text = m_Countdown.ToString();
        }
    }
}