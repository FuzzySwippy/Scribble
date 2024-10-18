using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.MagicSelection;

public partial class MagicSelectionProperties : ToolProperties
{
	[Export] private HSlider thresholdSlider;
	[Export] private Label thresholdLabel;

	public override void _Ready() => SetupControls();

	private void SetupControls()
	{
		thresholdSlider.MinValue = 0;
		thresholdSlider.MaxValue = 1;
		thresholdSlider.Step = 0.01f;
		thresholdSlider.ValueChanged += OnThresholdChanged;
	}

	private void OnThresholdChanged(double value)
	{
		((MagicSelectionTool)Tool).Threshold = (float)value;
		thresholdLabel.Text = $"{value:0.00}";
	}

	public override void UpdateProperties() => OnThresholdChanged(thresholdSlider.Value);
}
