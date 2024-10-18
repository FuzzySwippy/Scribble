using Godot;
using Scribble.Application;

namespace Scribble.UI.Info;

public partial class VersionLabel : Label
{
	public override void _Ready() => Text = Global.Version;
}
