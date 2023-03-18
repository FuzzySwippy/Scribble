using Godot;
using System;
using ScribbleLib.Input;

namespace Scribble;

public partial class CameraController : Camera2D
{
    static CameraController current;
    public static Vector2 CameraZoom
    {
        get => current.Zoom;
        set
        {
            current.Zoom = value;
            current.WindowSizeChanged();
        }
    }

    readonly float normalZoom = 0.475f;
    float zoomMin = 0.35f, zoomMax = 128;

    float speed = 500;
    bool isDragging = false;

    Rect2 ViewportRect { get; set; }
    Rect2 ViewportRectZoomed { get; set; }
    Rect2 Bounds { get; set; }
    Vector2 DistanceToSpacerEnd { get; set; }

    public CameraController() => current = this;

    public override void _Ready()
    {
        Mouse.Drag += MouseDrag;
        Mouse.Scroll += MouseScroll;
        Main.Ready += () =>
        {
            Main.Window.SizeChanged += WindowSizeChanged;
            CameraZoom = Zoom; //Update zoom and all associated values (ie. ViewportRectZoomed, Spacer.Rect) when the window value is set in Main
        };

        DebugInfo.Set("cam_zoom", CameraZoom.X);
    }

    public override void _Process(double delta) => DebugInfo.Set("cam_pos", Position);

    void WindowSizeChanged()
    {
        ViewportRect = GetViewportRect();
        ViewportRectZoomed = new(ViewportRect.Position / CameraZoom, ViewportRect.Size / CameraZoom);

        Spacer.UpdateRect();
        GD.Print(Spacer.ScaledRect);
        GD.Print(ViewportRectZoomed);
        LimitPosition();
    }

    void LimitPosition()
    {
        Bounds = new(Position - (ViewportRectZoomed.Size / 2), ViewportRectZoomed.Size);
        DistanceToSpacerEnd = Canvas.SizeInWorld + (ViewportRectZoomed.End - Spacer.ScaledRect.End);

        if (Spacer.ScaledRect.Size.X > Canvas.SizeInWorld.X * 1.5f)
        {
            if (Bounds.Position.X > -Spacer.ScaledRect.Position.X)
                Position = new(Bounds.Size.X / 2 - Spacer.ScaledRect.Position.X, Position.Y);
            if (Bounds.End.X < DistanceToSpacerEnd.X)
                Position = new(DistanceToSpacerEnd.X - Bounds.Size.X / 2, Position.Y);
        }
        else
        {
            if (Position.X < 0)
                Position = new(0, Position.Y);
            if (Position.X > Canvas.SizeInWorld.X)
                Position = new(Canvas.SizeInWorld.X, Position.Y);
        }

        if (Spacer.ScaledRect.Size.Y > Canvas.SizeInWorld.Y * 1.5f)
        {
        if (Bounds.Position.Y > -Spacer.ScaledRect.Position.Y)
            Position = new(Position.X, Bounds.Size.Y / 2 - Spacer.ScaledRect.Position.Y);
        if (Bounds.End.Y < DistanceToSpacerEnd.Y)
            Position = new(Position.X, DistanceToSpacerEnd.Y - Bounds.Size.Y / 2);
        }
        else
        {
            if (Position.Y < 0)
                Position = new(Position.X, 0);
            if (Position.Y > Canvas.SizeInWorld.Y)
                Position = new(Position.X, Canvas.SizeInWorld.Y);
        }
    }

    void MouseDrag(MouseButton button, Vector2 position, Vector2 change, Vector2 velocity)
    {
        if (button == MouseButton.Middle)
        {
            GlobalPosition -= change / CameraZoom;
            LimitPosition();
        }
    }

    void MouseScroll(int delta)
    {
        Vector2 newZoom = (CameraZoom * MathF.Pow(1.1f, delta)).Clamp(Vector2.One * zoomMin, Vector2.One * zoomMax);

        GlobalPosition = (GlobalPosition - GetGlobalMousePosition()) * CameraZoom / newZoom + GetGlobalMousePosition();
        CameraZoom = newZoom;
        LimitPosition();

        DebugInfo.Set("cam_zoom", CameraZoom.X);
    }
}
