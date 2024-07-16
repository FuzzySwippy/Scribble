using Godot;

namespace Scribble.UI;

public partial class GridWindow : Control
{
	[ExportGroup("Settings")]
	[Export] private CheckButton gridEnabled;
	[Export] private OptionButton gridColor;
	[Export] private OptionButton gridInterval;

	[ExportGroup("Buttons")]
	[Export] private Button okButton;
	[Export] private Button applyButton;
	[Export] private Button defaultButton;
	[Export] private Button cancelButton;


	public override void _Ready()
	{

	}
}
