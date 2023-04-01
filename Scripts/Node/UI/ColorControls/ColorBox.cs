using Godot;

namespace Scribble;

public partial class ColorBox : Control
{
    Control target;
    Control selector;
    TextureRect selectorTextureRect;
    Gradient baseColorGradient;
    InputEventMouseMotion mouseEvent;

    new Vector2 Position
    {
        get => selector.Position;
        set => selector.Position = value;
    }
    Vector2 MinPosition { get => target.Position; }
    Vector2 MaxPosition { get => target.Position + target.Size; }

    new Vector2 Size { get => MaxPosition - MinPosition; }

    Vector2 NormalizedPos { get => Position - MinPosition; }
    Vector2 NormalizedMaxPos { get => MaxPosition - MinPosition; }

    public float SValue { get => NormalizedPos.X / NormalizedMaxPos.X; }
    public float VValue { get => 1f - NormalizedPos.Y / NormalizedMaxPos.Y; }

    public override void _Ready()
    {
        target = GetChild<Control>(0);
        selector = GetChild<Control>(2);
        selectorTextureRect = selector.GetChild<TextureRect>(0);
        baseColorGradient = ((GradientTexture2D)GetChild<TextureRect>(0).Texture).Gradient;

        Main.Ready += UpdateHue;
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

                Global.ColorController.SetColorFromHueAndColorBox();
            }
        }
    }

    public void UpdateVisualization()
    {
        UpdateHue();
        Position = new(Global.ColorController.Color.S * Size.X + MinPosition.X, MaxPosition.Y - Global.ColorController.Color.V * Size.Y);
    }

    public void UpdateHue() => baseColorGradient.SetColor(1, Color.FromHsv(Global.ColorController.Color.H, 1, 1, Global.ColorController.Color.A));
}
