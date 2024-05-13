using Godot;

namespace Scribble;

public partial class DrawingCanvas : Node
{
	public override void _Ready() => Global.DrawingCanvas = this;
}
