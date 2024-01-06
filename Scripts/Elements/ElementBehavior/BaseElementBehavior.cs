using Godot;

public partial class BaseElementBehavior : Node2D
{
    protected Element m_element;

    public virtual void InitBehavior(Element el) { m_element = el; }
    public virtual void ClearBehavior() {}
}