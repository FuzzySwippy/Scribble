using System.Net.Sockets;
using Godot;
using System;
using ScribbleLib.Input;
using Scribble;

namespace Scribble;

public partial class CameraController : Camera2D
{
    readonly float normalZoom = 0.475f;
    float zoomMin = 0.35f, zoomMax = 128;

    float speed = 500;
    bool isDragging = false;

    public override void _Ready()
    {
        Mouse.Drag += MouseDrag;
        Mouse.Scroll += MouseScroll;
        Global.DebugInfo.Labels["cam_zoom"].Text = $"Camera zoom: {Zoom.X}";
    }

    public override void _Process(double delta)
    {
        Global.DebugInfo.Labels["cam_pos"].Text = $"Camera position: {Position}";
    }

    void MouseDrag(MouseButton button, Vector2 position, Vector2 change, Vector2 velocity)
    {
        if (button == MouseButton.Middle)
            GlobalPosition -= change / Zoom;
    }

    void MouseScroll(int delta)
    {
        Vector2 newZoom = (Zoom * MathF.Pow(1.1f, delta)).Clamp(Vector2.One * zoomMin, Vector2.One * zoomMax);
        //if (delta > 0)
            GlobalPosition = (GlobalPosition - GetGlobalMousePosition()) * Zoom / newZoom + GetGlobalMousePosition();
        Zoom = newZoom;

        Global.DebugInfo.Labels["cam_zoom"].Text = $"Camera zoom: {Zoom.X}";
    }
}
