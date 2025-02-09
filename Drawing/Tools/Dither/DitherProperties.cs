using Godot;
using Scribble.Drawing.Tools.Properties;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Pencil;

public partial class DitherProperties : ToolProperties
{
	[Export] private SpinBox sizeSpinBox;
	[Export] private OptionButton blendModeOptionButton;
	[Export] private OptionButton typeOptionButton;

	public override void _Ready() =>
		SetupControls();

	private void SetupControls()
	{
		sizeSpinBox.MinValue = Brush.MinSize;
		sizeSpinBox.MaxValue = Brush.MaxSize;
		sizeSpinBox.ValueChanged += OnSizeChanged;
		Brush.SizeChanged += size => sizeSpinBox.Value = size;
		Brush.BlendModeChanged += blendType => blendModeOptionButton.Selected = (int)blendType;

		blendModeOptionButton.AddEnumOptions<BlendMode>();

		blendModeOptionButton.ItemSelected += OnBlendModeSelected;
		typeOptionButton.ItemSelected += OnTypeSelected;
	}

	private void OnBlendModeSelected(long index) => Brush.BlendMode = (BlendMode)index;
	private void OnTypeSelected(long index) => ((DitherTool)Tool).Type = (ShapeType)index;
	private void OnSizeChanged(double value) => Brush.Size = (int)value;

	public override void UpdateProperties()
	{
		OnBlendModeSelected(blendModeOptionButton.Selected);
		OnTypeSelected(typeOptionButton.Selected);
		OnSizeChanged(sizeSpinBox.Value);
	}
}
