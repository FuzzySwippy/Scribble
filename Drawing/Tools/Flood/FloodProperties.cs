using Godot;
using Scribble.Drawing.Tools.Properties;

namespace Scribble.Drawing.Tools.Flood;

public partial class FloodProperties : ToolProperties
{
	[ExportGroup("Threshold")]
	[Export] private HSlider thresholdSlider;
	[Export] private Label thresholdLabel;

	[ExportGroup("Diagonal")]
	[Export] private CheckButton diagonalCheckButton;

	[ExportGroup("MergeLayers")]
	[Export] private CheckButton mergeLayersCheckButton;

	public override void _Ready() => SetupControls();

	private void SetupControls()
	{
		thresholdSlider.MinValue = 0;
		thresholdSlider.MaxValue = 1;
		thresholdSlider.Step = 0.01f;
		thresholdSlider.ValueChanged += OnThresholdChanged;

		diagonalCheckButton.Pressed += OnDiagonalPressed;

		mergeLayersCheckButton.Pressed += OnMergeLayersPressed;
	}

	private void OnThresholdChanged(double value)
	{
		((FloodTool)Tool).Threshold = (float)value;
		thresholdLabel.Text = $"{value:0.00}";
	}

	private void OnDiagonalPressed() =>
		((FloodTool)Tool).Diagonal = diagonalCheckButton.ButtonPressed;

	private void OnMergeLayersPressed() =>
		((FloodTool)Tool).MergeLayers = mergeLayersCheckButton.ButtonPressed;


	public override void UpdateProperties()
	{
		OnThresholdChanged(thresholdSlider.Value);
		OnDiagonalPressed();
		OnMergeLayersPressed();
	}
}
