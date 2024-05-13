using Godot;

namespace Scribble.UI;

public partial class ToolButton : Button
{
	public override void _Toggled(bool buttonPressed)
	{
		if (!buttonPressed)
			return;

		GD.Print(Name);
	}
}
