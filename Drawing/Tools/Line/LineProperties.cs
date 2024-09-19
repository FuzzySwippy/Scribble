using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Line;

public partial class LineProperties : ToolProperties
{
	[Export] private SpinBox sizeSpinBox;
	[Export] private OptionButton typeOptionButton;

	public override void _Ready()
	{
		SetupControls();
	}

	private void SetupControls()
	{
		sizeSpinBox.MinValue = Brush.MinSize;
		sizeSpinBox.MaxValue = Brush.MaxSize;
		sizeSpinBox.ValueChanged += OnSizeChanged;
		Brush.SizeChanged += size => sizeSpinBox.Value = size;

		typeOptionButton.ItemSelected += OnTypeSelected;
	}

	private void OnTypeSelected(long index) => ((LineTool)Tool).Type = (ShapeType)index;
	private void OnSizeChanged(double value) => Brush.Size = (int)value;

	public override void UpdateProperties()
	{
		OnTypeSelected(typeOptionButton.Selected);
		OnSizeChanged(sizeSpinBox.Value);
	}
}
