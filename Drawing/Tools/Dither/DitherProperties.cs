using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Pencil;

public partial class DitherProperties : ToolProperties
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
	private void OnTypeSelected(long index) => ((DitherTool)Tool).Type = (ShapeType)index;
	private void OnSizeChanged(double value) => Brush.Size = (int)value;

	public override void UpdateProperties()
	{
		OnBlendTypeSelected(blendTypeOptionButton.Selected);
		OnTypeSelected(typeOptionButton.Selected);
		OnSizeChanged(sizeSpinBox.Value);
	}
}
