using Godot;
using Colors = ScribbleLib.Colors;

namespace Scribble;

public partial class ColorBox : Control
{
    Control target;
    Control selector;
    Gradient baseColorGradient;
    InputEventMouseMotion mouseEvent;
    readonly Vector2 margin = new(5, 5);

    new Vector2 Position
    {
        get => selector.Position;
        set => selector.Position = value;
    }
    Vector2 MinPosition { get => target.Position + margin; }
    Vector2 MaxPosition { get => target.Position + target.Size - margin; }

    Vector2 NormalizedPos { get => Position - MinPosition; }
    Vector2 NormalizedMaxPos { get => MaxPosition - MinPosition; }

    public float XValue { get => NormalizedPos.X / NormalizedMaxPos.X; }
    public float YValue { get => NormalizedPos.Y / NormalizedMaxPos.Y; }

    public override void _Ready()
    {
        target = GetChild<Control>(0);
        selector = GetChild<Control>(2);
        baseColorGradient = ((GradientTexture2D)GetChild<TextureRect>(0).Texture).Gradient;

        Main.Ready += () => UpdateBaseColor(Global.HueSlider.Color);
    }

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            mouseEvent = (InputEventMouseMotion)e;
            if (mouseEvent.ButtonMask == MouseButtonMask.Left)
            {
                Position = mouseEvent.Position;
                Position = Position.Clamp(MinPosition, MaxPosition);
                Global.ColorController.UpdatePencilColor();
            }
        }
    }

    public void UpdateBaseColor(Color color) => baseColorGradient.SetColor(1, color);
}
