using Godot;

public partial class BaseElementEffect : Node2D
{
    protected Element m_elementParent;

    public virtual void InitEffect(Element el) { m_elementParent = el; }

    public virtual void OnMatchEffect(ElementsMatch match) {}

    public virtual void ClearEffect() {}
}