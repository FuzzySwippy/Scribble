using Godot;
using Godot.Collections;
using Scribble.Application;

namespace Scribble.UI;

public partial class AboutWindow : Node
{
	[Export] private Label versionLabel;

	[Export] private Array<RichTextLabel> urlLabels;

	public override void _Ready()
	{
		versionLabel.Text = $"Version: {Global.Version}";

		foreach (RichTextLabel urlLabel in urlLabels)
			urlLabel.MetaClicked += meta => OS.ShellOpen(meta.ToString());
	}
}
