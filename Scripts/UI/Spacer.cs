using Godot;

namespace Scribble;

public partial class Spacer : Control
{
    public static bool MouseInBounds { get; private set; } = false;
    public static Rect2 Rect { get; private set; }
    public static Rect2 ScaledRect { get; private set; }

    public override void _Ready()
    {
        Global.Spacer = this;

        MouseEntered += () => MouseInBounds = true;
        MouseExited += () => MouseInBounds = false;
        Resized += UpdateRect;
    }

    public static void UpdateRect()
    {
        Rect = new(Global.Spacer.GlobalPosition, Global.Spacer.Size);
        ScaledRect = new(Rect.Position / CameraController.CameraZoom, Rect.Size / CameraController.CameraZoom);
    }
}
