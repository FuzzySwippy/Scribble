using Godot;

namespace Scribble;

public partial class ColorBox : Control
{
    public ColorInput ColorInput { get; set; }

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

                ColorInput.SetColorFromHueAndColorBox();
            }
        }
    }

    public void UpdateVisualization()
    {
        UpdateHue();
        Position = new(ColorInput.Color.S * Size.X + MinPosition.X, MaxPosition.Y - ColorInput.Color.V * Size.Y);
    }

    public void UpdateHue() => baseColorGradient.SetColor(1, Color.FromHsv(ColorInput.Color.H, 1, 1, 1));
}
