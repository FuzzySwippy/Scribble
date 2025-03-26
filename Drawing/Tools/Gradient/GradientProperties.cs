using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Gradient;

public partial class GradientProperties : ToolProperties
{
	[Export] private OptionButton blendModeOptionButton;
	[Export] private OptionButton typeOptionButton;

	public override void _Ready() =>
		SetupControls();

	private void SetupControls()
	{
		Brush.BlendModeChanged += blendType => blendModeOptionButton.Selected = (int)blendType;

		blendModeOptionButton.AddEnumOptions<BlendMode>();

		blendModeOptionButton.ItemSelected += OnBlendModeSelected;
		typeOptionButton.ItemSelected += OnTypeSelected;
	}

	private void OnBlendModeSelected(long index) => Brush.BlendMode = (BlendMode)index;
	private void OnTypeSelected(long index) => ((GradientTool)Tool).Type = (GradientType)index;

	public override void UpdateProperties()
	{
		OnTypeSelected(typeOptionButton.Selected);
		OnBlendModeSelected(blendModeOptionButton.Selected);
	}
}
