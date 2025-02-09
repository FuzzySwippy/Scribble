using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Rectangle;

public partial class RectangleProperties : ToolProperties
{
	[Export] private OptionButton blendModeOptionButton;

	public override void _Ready() =>
		SetupControls();

	private void SetupControls()
	{
		blendModeOptionButton.AddEnumOptions<BlendMode>();
		blendModeOptionButton.ItemSelected += OnBlendModeSelected;
		Brush.BlendModeChanged += blendType => blendModeOptionButton.Selected = (int)blendType;
	}

	private void OnBlendModeSelected(long index) => Brush.BlendMode = (BlendMode)index;

	public override void UpdateProperties() =>
		OnBlendModeSelected(blendModeOptionButton.Selected);
}
