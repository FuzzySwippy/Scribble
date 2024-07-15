using Godot;
using Godot.Collections;

namespace Scribble.UI;

public partial class AboutWindow : Node
{
	[Export] private Label menuBarVersionLabel;
	[Export] private Label versionLabel;

	[Export] private Array<RichTextLabel> urlLabels;

	public override void _Ready()
	{
		versionLabel.Text = $"Version: {menuBarVersionLabel.Text}";

		foreach (RichTextLabel urlLabel in urlLabels)
			urlLabel.MetaClicked += meta => OS.ShellOpen(meta.ToString());
	}
}
