using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorComponentSlider : Control
{
    [Export]
    public ColorComponent Component { get; set; }

    Gradient gradient;

    readonly float sliderMargin = 3;
    int newValue;
    int oldCaretPos;
    bool ignoreInputUpdate;

    SpinBox valueInput;
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
        valueInput = GetChild(0).GetChild(0).GetChild<SpinBox>(1);
        slider = GetChild<HSlider>(1);
        grabber = GetChild(1).GetChild<Button>(0);

        valueInput.ValueChanged += ValueChanged;
        slider.ValueChanged += ValueChanged;
        slider.Resized += Global.ColorController.SetColorFromComponentSliders;

        gradient = ((GradientTexture1D)((StyleBoxTexture)GetChild<HSlider>(1).GetThemeStylebox("slider")).Texture).Gradient;

        Main.Ready += UpdateGradient;
    }

    void ValueChanged(double value)
    { 
        if (ignoreInputUpdate)
            return;

        Global.ColorController.SetColorFromComponentSliders();
    }

    void UpdateGrabber() => grabber.Position = new(Value * slider.Size.X - sliderMargin, grabber.Position.Y);

    public void UpdateGradient()
    {
        gradient.SetColor(0, Component switch
        {
            ColorComponent.R => Global.ColorController.Color.Color.SetR(0),
            ColorComponent.G => Global.ColorController.Color.Color.SetG(0),
            ColorComponent.B => Global.ColorController.Color.Color.SetB(0),
            _ => Global.ColorController.Color.Color.SetA(0)
        });

        gradient.SetColor(1, Component switch
        {
            ColorComponent.R => Global.ColorController.Color.Color.SetR(1),
            ColorComponent.G => Global.ColorController.Color.Color.SetG(1),
            ColorComponent.B => Global.ColorController.Color.Color.SetB(1),
            _ => Global.ColorController.Color.Color.SetA(1)
        });
    }

    public override void _Process(double delta)
    {
        if (Component != ColorComponent.A)
            return;

        DebugInfo.Set("debug", valueInput.Value);
    }
}