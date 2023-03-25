using System.Text;
using Godot;

namespace Scribble;

public partial class ColorValueSlider : Control
{
    readonly float sliderMargin = 2;
    readonly StringBuilder rebuiltString = new();
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
            slider.Value = value;
            valueInput.Value = (int)(value * 255);
        }
    }

    public override void _Ready()
    {
        valueInput = GetChild(0).GetChild(0).GetChild<SpinBox>(1);
        slider = GetChild<HSlider>(1);
        grabber = GetChild(1).GetChild<Button>(0);

        valueInput.ValueChanged += InputValueChanged;
        slider.ValueChanged += SliderValueChanged;
    }

    void InputValueChanged(double value)
    {
        if (ignoreInputUpdate)
        {
            ignoreInputUpdate = false;
            return;
        }

        ignoreInputUpdate = true;
        slider.Value = value / 255;

        //Global.ColorBox.UpdateBaseColor(Color);
        //Global.ColorController.UpdatePencilColor();
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

        //Global.ColorBox.UpdateBaseColor(Color);
        //Global.ColorController.UpdatePencilColor();
    }
}
