using Godot;

namespace Scribble;

public partial class HueSlider : VSlider
{
    public float HValue
    {
        get => 1f - (float)Value;
        private set => Value = 1f - value;
    }

    bool ignoreUpdate = false;
    Button grabber;

    public override void _Ready()
    {
        grabber = GetChild<Button>(0);
        GradientSetup();
    }

    public override void _ValueChanged(double newValue)
    {
        grabber.Position = new(grabber.Position.X, Size.Y - (float)newValue * Size.Y);
        if (ignoreUpdate)
        {
            ignoreUpdate = false;
            return;
        }

        Global.ColorController.SetColorFromHueAndColorBox();
    }

    public void UpdateVisualization()
    {
        ignoreUpdate = true;
        HValue = Global.ColorController.Color.H;
    }

    static void GradientSetup()
    {
        Gradient gradient = Global.HueSliderTexture.Gradient;
        float step = 1f / 6;

        //Removing all point generates an error so the first point has to be set outside of the loop
        gradient.SetColor(0, Color.FromHsv(0, 1, 1));
        gradient.RemovePoint(1);

        for (float i = step; i <= 1; i += step)
            gradient.AddPoint(i, Color.FromHsv(i, 1, 1));
    }
}
