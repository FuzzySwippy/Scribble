using Godot;
using Scribble.Drawing;

namespace Scribble.UI;

public partial class ToolButton : Button
{
	[Export] public DrawingToolType ToolType { get; set; }

	private TextureRect SelectedTexture { get; set; }
	private DrawingToolPanel DrawingToolPanel { get; set; }

	public override void _Ready()
	{
		SelectedTexture = GetNode<TextureRect>("SelectedTexture");
		Deselect();
	}

	public void Init(DrawingToolPanel panel) => DrawingToolPanel = panel;

	public override void _Pressed() => DrawingToolPanel.Select(ToolType);

	public void Select() => SelectedTexture.Show();

	public void Deselect() => SelectedTexture.Hide();
}
