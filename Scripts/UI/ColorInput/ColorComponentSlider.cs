using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorComponentSlider : Node
{
    public ColorInput ColorInput { get; set; }

    [Export]
    public ColorComponent Component { get; set; }

    Gradient gradient;

    readonly float sliderMargin = 3;
    int newValue;
    int oldCaretPos;
    bool ignoreInputUpdate;

    SpinBox valueInput;
    TextureRect transparencyBackground;
    HSlider slider;
    Button grabber;

    public float Value
    {
        get => (float)slider.Value;
        set
        {
            ignoreInputUpdate = true;
            valueInput.Value = (int)(value * 255);
            slider.Value = value;
            ignoreInputUpdate = false;

            UpdateGrabber();
            UpdateGradient();
        }
    }

    public override void _Ready()
    {
        GD.Print("ColorComponentSlider ready");

        valueInput = GetChild(0).GetChild(0).GetChild<SpinBox>(1);
        transparencyBackground = GetChild(1).GetChild<TextureRect>(0);
        slider = GetChild(1).GetChild<HSlider>(1);
        grabber = slider.GetChild<Button>(0);

        valueInput.ValueChanged += ValueChanged;
        slider.ValueChanged += ValueChanged;


        StyleBoxTexture styleBox = (StyleBoxTexture)Global.ColorComponentStyleBox.Duplicate(true);
        slider.AddThemeStyleboxOverride("slider", styleBox);
        gradient = ((GradientTexture1D)styleBox.Texture).Gradient;

        if (Component == ColorComponent.A)
            transparencyBackground.Texture = TextureGenerator.NewBackgroundTexture(new(28, 3));

        Main.Ready += UpdateGradient;
        slider.Resized += UpdateGrabber;
    }

    void ValueChanged(double value)
    { 
        if (ignoreInputUpdate)
            return;

        ColorInput.SetColorFromComponentSliders();
    }

    void UpdateGrabber() => grabber.Position = new(Value * slider.Size.X - sliderMargin, grabber.Position.Y);

    public void UpdateGradient()
    {
        gradient.SetColor(0, Component switch
        {
            ColorComponent.R => ColorInput.Color.GDColorOpaque.SetR(0),
            ColorComponent.G => ColorInput.Color.GDColorOpaque.SetG(0),
            ColorComponent.B => ColorInput.Color.GDColorOpaque.SetB(0),
            _ => ColorInput.Color.GDColor.SetA(0)
        });

        gradient.SetColor(1, Component switch
        {
            ColorComponent.R => ColorInput.Color.GDColorOpaque.SetR(1),
            ColorComponent.G => ColorInput.Color.GDColorOpaque.SetG(1),
            ColorComponent.B => ColorInput.Color.GDColorOpaque.SetB(1),
            _ => ColorInput.Color.GDColor.SetA(1)
        });
    }
}