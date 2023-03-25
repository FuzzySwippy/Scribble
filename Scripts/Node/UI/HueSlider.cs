using Godot;

namespace Scribble;

public partial class HueSlider : VSlider
{
    public Color Color
    {
        get => Color.FromHsv(1 - (float)Value, 1, 1);
        private set => Value = value.H;
    }

    Button grabber;

    public override void _Ready()
    {
        grabber = GetChild<Button>(0);
        GradientSetup();
    }

    public override void _ValueChanged(double newValue)
    {
        grabber.Position = new(grabber.Position.X, Size.Y - (float)newValue * Size.Y);
        Global.ColorBox.UpdateBaseColor(Color);
        Global.ColorController.UpdatePencilColor();
    }

    static void GradientSetup()
    {
        Gradient gradient = Global.HueSliderTexture.Gradient;
        float step = 1f / 6;

        gradient.RemovePoint(1);
        gradient.RemovePoint(0);

        for (float i = 0; i <= 1; i += step)
            gradient.AddPoint(i, Color.FromHsv(i, 1, 1));
    }
}
