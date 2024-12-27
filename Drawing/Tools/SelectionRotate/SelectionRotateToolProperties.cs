using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Pencil;

public partial class SelectionRotateToolProperties : ToolProperties
{
	[Export] private CheckButton interpolateEmptyPixelsCheckButton;

	public override void _Ready() =>
		interpolateEmptyPixelsCheckButton.Toggled += OnInterpolateEmptyPixelsToggled;

	private void OnInterpolateEmptyPixelsToggled(bool value) =>
		((SelectionRotateTool)Tool).InterpolateEmptyPixels = value;

	public override void UpdateProperties() =>
		OnInterpolateEmptyPixelsToggled(interpolateEmptyPixelsCheckButton.ButtonPressed);
}
