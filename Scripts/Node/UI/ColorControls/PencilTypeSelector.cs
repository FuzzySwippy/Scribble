using Godot;

namespace Scribble;

public partial class PencilTypeSelector : Control
{
    TextureRect selectorRect;
    ColorRect colorRect;
    InputEventMouseButton mouseEvent;

    
    [Export]
	public PencilType Type { get; set; }

    [Export(PropertyHint.MultilineText)]
    string ToolTip { get; set; }

    public new bool Visible
    {
        get => selectorRect.Visible;
        set => selectorRect.Visible = value;
    }

    public override void _Ready()
    {
        selectorRect = GetChild(0).GetChild(1).GetChild<TextureRect>(0);
        selectorRect.TooltipText = ToolTip;
        Visible = false;
        Global.ColorController.PencilTypeSelectors.Add(this);

        colorRect = GetChild(0).GetChild<ColorRect>(1);
        colorRect.GuiInput += GuiInputEvent;
        colorRect.TooltipText = ToolTip;

        Main.Ready += UpdateColor;
    }

    void GuiInputEvent(InputEvent e)
    {
        if (e is InputEventMouseButton)
        {
            mouseEvent = (InputEventMouseButton)e;
            if (mouseEvent.ButtonMask != MouseButtonMask.Left)
                return;

            if (!Visible)
                Global.ColorController.SelectedType = Type;
        }
    }

    public void UpdateColor()
    {
        GD.Print($"{Main.Artist.Brush.PencilColor(Type)}{System.Environment.NewLine}{Main.Artist.Brush.PencilColor(Type).Color}");
        colorRect.Color = Main.Artist.Brush.PencilColor(Type).Color;
    }

    public void SetBackground(Texture2D texture) => GetChild(0).GetChild<TextureRect>(0).Texture = texture;
}
