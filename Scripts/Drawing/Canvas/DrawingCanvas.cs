using Godot;

namespace Scribble;

public partial class DrawingCanvas : Node
{
	public static DrawingCanvas Current { get; private set; }

	public override void _Ready() => Current = this;
}
