using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Rectangle;

public partial class RectangleProperties : ToolProperties
{
	[Export] private OptionButton blendTypeOptionButton;

	public override void _Ready() =>
		SetupControls();

	private void SetupControls()
	{
		blendTypeOptionButton.AddEnumOptions<BlendType>();
		blendTypeOptionButton.ItemSelected += OnBlendTypeSelected;
		Brush.BlendTypeChanged += blendType => blendTypeOptionButton.Selected = (int)blendType;
	}

	private void OnBlendTypeSelected(long index) => Brush.BlendType = (BlendType)index;

	public override void UpdateProperties() =>
		OnBlendTypeSelected(blendTypeOptionButton.Selected);
}
