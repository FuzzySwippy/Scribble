using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class HueSlider : VSlider
{
	public ColorInput Parent { get; set; }

	private bool ignoreUpdate = false;
	private Control grabber;
	public float HValue
	{
		get => 1f - (float)Value;
		private set => Value = 1f - value;
	}

	private bool UpdateGrabberOnDraw { get; set; }

	public override void _Ready() =>
		grabber = GetChild<Control>(0);

	public override void _ValueChanged(double newValue)
	{
		if (Size.Y == 0)
		{
			UpdateGrabberOnDraw = true;
			return;
		}

		grabber.Position = new(grabber.Position.X, Size.Y - (float)newValue * Size.Y);
		if (ignoreUpdate)
		{
			ignoreUpdate = false;
			return;
		}

		Parent.SetColorFromHueAndColorBox();
	}

	public override void _Draw()
	{
		if (UpdateGrabberOnDraw)
		{
			UpdateGrabberOnDraw = false;
			_ValueChanged(Value);
		}
	}

	public void UpdateVisualization()
	{
		ignoreUpdate = true;
		HValue = Parent.Color.H;
	}

	public static void GradientSetup()
	{
		Gradient gradient = ((GradientTexture2D)Global.HueSliderStyleBox.Texture).Gradient;
		float step = 1f / 6;

		//Removing all points generates an error so the first point has to be set outside of the loop
		gradient.SetColor(0, Color.FromHsv(0, 1, 1));
		gradient.RemovePoint(1);

		for (float i = step; i <= 1; i += step)
			gradient.AddPoint(i, Color.FromHsv(i, 1, 1));
	}
}
