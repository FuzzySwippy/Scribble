using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorComponentSlider : Control
{
    [Export]
    public ColorComponent Component { get; set; }

    Gradient gradient;

    readonly float sliderMargin = 2;
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
        UpdateGradient();
    }

    void ValueChanged(double value)
    { 
        if (ignoreInputUpdate)
            return;

        Global.ColorController.SetColorFromComponentSliders();
    }

    void UpdateGrabber()
    {
        grabber.Position = new(Value * slider.Size.X - sliderMargin, grabber.Position.Y);
        GD.Print($"{Value} {slider.Size} {grabber.Position}");
    }

    /*void InputValueChanged(double value)
    {
        if (ignoreInputUpdate)
        {
            ignoreInputUpdate = false;
            return;
        }

        ignoreInputUpdate = true;
        slider.Value = value / 255;
    }

    void SliderValueChanged(double value)
    {
        grabber.Position = new((float)value * slider.Size.X - sliderMargin, grabber.Position.Y);
		
        if (ignoreInputUpdate)
        {
            ignoreInputUpdate = false;
            return;
        }

        ignoreInputUpdate = true;
        valueInput.Value = (int)(value * 255);

        Global.ColorController.SetColorFromComponentSliders();
    }*/

    public void UpdateGradient()
    {
        Color color1 = new(0, 0, 0, Component == ColorComponent.A ? 0 : 1);
        Color color2 = ScribbleColors.ComponentMap[Component];

        gradient.SetColor(0, color1);
        gradient.SetColor(1, color2);
    }

    public override void _Process(double delta)
    {
        if (Component != ColorComponent.A)
            return;

        DebugInfo.Set("debug", valueInput.Value);
    }
}