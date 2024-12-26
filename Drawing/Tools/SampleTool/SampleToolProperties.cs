using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Pencil;

public partial class SampleToolProperties : ToolProperties
{
	[Export] private CheckButton ignoreLayerOpacityCheckButton;
	[Export] private CheckButton mergeLayersCheckButton;

	public override void _Ready()
	{
		ignoreLayerOpacityCheckButton.Toggled += OnIgnoreLayerOpacityToggled;
		mergeLayersCheckButton.Toggled += OnMergeLayersToggled;
	}

	private void OnIgnoreLayerOpacityToggled(bool value) => ((SampleTool)Tool).IgnoreLayerOpacity = value;
	private void OnMergeLayersToggled(bool value) => ((SampleTool)Tool).MergeLayers = value;

	public override void UpdateProperties()
	{
		OnIgnoreLayerOpacityToggled(ignoreLayerOpacityCheckButton.ButtonPressed);
		OnMergeLayersToggled(mergeLayersCheckButton.ButtonPressed);
	}
}
