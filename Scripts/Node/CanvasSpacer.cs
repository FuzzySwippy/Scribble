using Godot;

namespace Scribble;

public partial class CanvasSpacer : Control
{
    public static bool MouseInBounds { get; private set; } = false;

    public override void _Ready()
    {
        MouseEntered += () => MouseInBounds = true;
        MouseExited += () => MouseInBounds = false;
    }
}
