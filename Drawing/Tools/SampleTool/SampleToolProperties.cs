using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Pencil;

public partial class SampleToolProperties : ToolProperties
{
	[Export] private CheckButton ignoreLayerOpacityCheckButton;

	public override void _Ready() => ignoreLayerOpacityCheckButton.Toggled += OnIgnoreLayerOpacityToggled;

	private void OnIgnoreLayerOpacityToggled(bool value) => ((SampleTool)Tool).IgnoreLayerOpacity = value;

	public override void UpdateProperties() => OnIgnoreLayerOpacityToggled(ignoreLayerOpacityCheckButton.ButtonPressed);
}
