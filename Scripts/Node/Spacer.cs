using Godot;

namespace Scribble;

public partial class Spacer : Control
{
    public static bool MouseInBounds { get; private set; } = false;
    public static Rect2 ScaledRect { get; private set; }

    public override void _Ready()
    {
        MouseEntered += () => MouseInBounds = true;
        MouseExited += () => MouseInBounds = false;
        Resized += UpdateRect;
    }

    public static void UpdateRect() => ScaledRect = new(Global.Spacer.GlobalPosition / CameraController.CameraZoom, Global.Spacer.Size / CameraController.CameraZoom);
}
