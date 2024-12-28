using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Pencil;

public partial class SelectionRotateToolProperties : ToolProperties
{
	[Export] private CheckButton interpolateEmptyPixelsCheckButton;
	[Export] private CheckButton ignoreEmptyColorsCheckButton;

	public override void _Ready()
	{
		interpolateEmptyPixelsCheckButton.Toggled += OnInterpolateEmptyPixelsToggled;
		ignoreEmptyColorsCheckButton.Toggled += OnIgnoreEmptyColorsToggled;
	}

	private void OnInterpolateEmptyPixelsToggled(bool value)
	{
		((SelectionRotateTool)Tool).InterpolateEmptyPixels = value;
		ignoreEmptyColorsCheckButton.Disabled = !value;
	}

	private void OnIgnoreEmptyColorsToggled(bool value) =>
		((SelectionRotateTool)Tool).IgnoreEmptyColors = value;

	public override void UpdateProperties()
	{
		OnInterpolateEmptyPixelsToggled(interpolateEmptyPixelsCheckButton.ButtonPressed);
		OnIgnoreEmptyColorsToggled(ignoreEmptyColorsCheckButton.ButtonPressed);
	}
}
