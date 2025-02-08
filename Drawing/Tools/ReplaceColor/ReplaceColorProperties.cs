using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Flood;

public partial class ReplaceColorProperties : ToolProperties
{
	[ExportGroup("AllFrames")]
	[Export] private CheckButton allFramesCheckButton;

	[ExportGroup("AllLayers")]
	[Export] private CheckButton allLayersCheckButton;

	[ExportGroup("IgnoreOpacity")]
	[Export] private CheckButton ignoreOpacityCheckButton;

	public override void _Ready() => SetupControls();

	private void SetupControls()
	{
		allFramesCheckButton.Pressed += OnAllFramesPressed;
		allLayersCheckButton.Pressed += OnAllLayersPressed;
		ignoreOpacityCheckButton.Pressed += OnIgnoreOpacityPressed;
	}

	private void OnAllFramesPressed()
	{
		((ReplaceColorTool)Tool).AllFrames = allFramesCheckButton.ButtonPressed;
		if (allFramesCheckButton.ButtonPressed)
			allLayersCheckButton.ButtonPressed = true;
	}

	private void OnAllLayersPressed()
	{
		if (allFramesCheckButton.ButtonPressed)
			allLayersCheckButton.ButtonPressed = true;

		((ReplaceColorTool)Tool).AllLayers = allLayersCheckButton.ButtonPressed;
	}

	private void OnIgnoreOpacityPressed() =>
		((ReplaceColorTool)Tool).IgnoreOpacity = ignoreOpacityCheckButton.ButtonPressed;

	public override void UpdateProperties()
	{
		OnAllFramesPressed();
		OnAllLayersPressed();
		OnIgnoreOpacityPressed();
	}
}
