using Godot;

namespace Scribble;

public partial class ColorBox : Control
{
    public ColorInput ColorInput { get; set; }

    Control target;
    Control selector;
    TextureRect selectorTextureRect;
    Gradient baseColorGradient;

    new Vector2 Position
    {
        get => selector.Position;
        set => selector.Position = value;
    }
    Vector2 MinPosition => target.Position;
    Vector2 MaxPosition => target.Position + target.Size;

    new Vector2 Size => MaxPosition - MinPosition;

    Vector2 NormalizedPos => Position - MinPosition;
    Vector2 NormalizedMaxPos => MaxPosition - MinPosition;

    public float SValue => NormalizedPos.X / NormalizedMaxPos.X;
    public float VValue => 1f - NormalizedPos.Y / NormalizedMaxPos.Y;

    public override void _Ready()
    {
        target = GetChild<Control>(0);
        selector = GetChild<Control>(2);
        selectorTextureRect = selector.GetChild<TextureRect>(0);
        SetUpGradient();

        Main.Ready += UpdateHue;
    }

    void SetUpGradient()
    {
        GradientTexture2D gradientTexture = (GradientTexture2D)Global.ColorBoxGradientTexture.Duplicate(true);
        GetChild<TextureRect>(0).Texture = gradientTexture;
        baseColorGradient = gradientTexture.Gradient;
    }

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseMotion motionEvent)
        {
            if (motionEvent.ButtonMask == MouseButtonMask.Left)
            {
                Position = motionEvent.Position;
                Position = Position.Clamp(MinPosition, MaxPosition);

                ColorInput.SetColorFromHueAndColorBox();
            }
        }
        else if (e is InputEventMouseButton buttonEvent)
        {
            if (buttonEvent.ButtonMask == MouseButtonMask.Left)
            {
                Position = buttonEvent.Position;
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
