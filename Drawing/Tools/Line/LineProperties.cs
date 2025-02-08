using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Line;

public partial class LineProperties : ToolProperties
{
	[Export] private SpinBox sizeSpinBox;
	[Export] private OptionButton blendTypeOptionButton;
	[Export] private OptionButton typeOptionButton;

	public override void _Ready() =>
		SetupControls();

	private void SetupControls()
	{
		sizeSpinBox.MinValue = Brush.MinSize;
		sizeSpinBox.MaxValue = Brush.MaxSize;
		sizeSpinBox.ValueChanged += OnSizeChanged;
		Brush.SizeChanged += size => sizeSpinBox.Value = size;
		Brush.BlendTypeChanged += blendType => blendTypeOptionButton.Selected = (int)blendType;

		blendTypeOptionButton.AddEnumOptions<BlendType>();

		blendTypeOptionButton.ItemSelected += OnBlendTypeSelected;
		typeOptionButton.ItemSelected += OnTypeSelected;
	}

	private void OnBlendTypeSelected(long index) => Brush.BlendType = (BlendType)index;
	private void OnTypeSelected(long index) => ((LineTool)Tool).Type = (ShapeType)index;
	private void OnSizeChanged(double value) => Brush.Size = (int)value;

	public override void UpdateProperties()
	{
		OnTypeSelected(typeOptionButton.Selected);
		OnBlendTypeSelected(blendTypeOptionButton.Selected);
		OnSizeChanged(sizeSpinBox.Value);
	}
}
