using System.Net.Sockets;
using Godot;
using System;
using ScribbleLib.Input;

public partial class CameraController : Camera2D
{
    float speed = 500;
    bool isDragging = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Mouse.Drag += MouseDrag;
        Mouse.Scroll += MouseScroll;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    void MouseDrag(MouseButton button, Vector2 position, Vector2 change, Vector2 velocity)
    {
        if (button == MouseButton.Left)
            GlobalPosition = new Vector2((GlobalPosition.X - change.X / Zoom.X), (GlobalPosition.Y - change.Y / Zoom.Y));
    }

    void MouseScroll(int delta)
    {
        Vector2 newZoom = Zoom * MathF.Pow(1.1f, delta);
        GlobalPosition = (GlobalPosition - GetGlobalMousePosition()) * Zoom / newZoom + GetGlobalMousePosition();
        Zoom = newZoom;
    }
}
