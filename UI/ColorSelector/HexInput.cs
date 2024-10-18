using Godot;

namespace Scribble.UI;

public partial class HexInput : Node
{
	public ColorInput ColorInput { get; set; }

	private LineEdit input;
	private Label label;
	private bool ignoreVisualizationUpdate = false;

	[Export] private Color invalidColor;

	public Color? Color { get; private set; }


	public override void _Ready()
	{
		label = GetChild<Label>(0);
		input = GetChild<LineEdit>(1);

		input.TextChanged += InputTextChanged;
	}

	private void InputTextChanged(string newText)
	{
		GD.Print("Hex Input Updated");

		try
		{
			Color = Godot.Color.FromHtml(newText);

			ignoreVisualizationUpdate = true;
			ColorInput.SetColorFromHexInput();

			input.RemoveThemeColorOverride("font_color");
		}
		catch (System.Exception)
		{
			Color = null;
			input.AddThemeColorOverride("font_color", invalidColor);
		}
	}

	public void UpdateVisualizations()
	{
		if (ignoreVisualizationUpdate)
		{
			ignoreVisualizationUpdate = false;
			return;
		}

		input.Text = $"#{ColorInput.Color.GodotColor.ToHtml()}";
	}
}
