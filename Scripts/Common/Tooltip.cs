using Godot;

public partial class Tooltip : Node2D
{
    [Export] public Label m_TitleLabel;
    [Export] public Label m_DescriptionLabel;

    public void SetVisible(bool val)
    {
        Visible = val;
    }

    public void SetTitle(string text)
    {
        m_TitleLabel.Text = text;
    }

    public void SetDescription(string text)
    {
        m_DescriptionLabel.Text = text;
    }

}
