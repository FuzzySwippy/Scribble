using Godot;
using System;
using ScribbleLib;
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
			current.Zoom = value.Clamp(MinZoom, MaxZoom);
			current.WindowSizeChanged();
		}
	}

	readonly float zoomMin = 0.35f, zoomMax = 96, normalZoom = 0.475f;

	public static Vector2 MinZoom => (current.zoomMin / (UI.ContentScale * 2)).ToVector2();
	public static Vector2 MaxZoom => (current.zoomMax / (UI.ContentScale * 2)).ToVector2();

	public static Vector2 ZoomAmount
	{
		get => (current.Zoom - MinZoom) / (MaxZoom - MinZoom);
		set => CameraZoom = (MaxZoom - MinZoom) * value + MinZoom;
	}

	public static Vector2 RelativePosition
	{
		get => current.GlobalPosition - (Canvas.SizeInWorld / 2);
		set => current.GlobalPosition = value + Canvas.SizeInWorld / 2;
	}

	bool isDragging = false;

	Rect2 ViewportRectZoomed { get; set; }
	Rect2 Bounds { get; set; }
	Vector2 DistanceToSpacerEnd { get; set; }

	public CameraController() => current = this;

	public override void _Ready()
	{
		Global.Camera = this;

		Mouse.Drag += MouseDrag;
		Mouse.ButtonUp += MouseUp;
		Mouse.Scroll += MouseScroll;
		Main.Ready += () => CameraZoom = Zoom; //Update zoom and all associated values (ie. ViewportRectZoomed, Spacer.Rect) when the window value is set in Main
		Main.WindowSizeChanged += WindowSizeChanged;

		DebugInfo.Set("cam_zoom", CameraZoom.X);
	}

	public override void _Process(double delta) => DebugInfo.Set("cam_pos", Position);

	void WindowSizeChanged()
	{
		ViewportRectZoomed = new(Main.ViewportRect.Position / CameraZoom, Main.ViewportRect.Size / CameraZoom);

		Spacer.UpdateRect();
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

	void MouseDrag(MouseCombination combination, Vector2 position, Vector2 change, Vector2 velocity)
	{
		if (combination.button == MouseButton.Middle)
		{
			GlobalPosition -= change / CameraZoom;
			LimitPosition();
			Mouse.WarpBorder = Spacer.Rect;
		}
	}

	void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (combination.button == MouseButton.Middle)
			Mouse.WarpBorder = new();
	}

	void MouseScroll(KeyModifierMask modifiers, int delta)
	{
		if (modifiers != 0)
			return;

		Vector2 newZoom = (CameraZoom * MathF.Pow(1.1f, delta)).Clamp(MinZoom, MaxZoom);

		//Restricts zoom-movement
		if (newZoom != MaxZoom)
			GlobalPosition = (GlobalPosition - GetGlobalMousePosition()) * CameraZoom / newZoom + GetGlobalMousePosition();
		CameraZoom = newZoom;
		LimitPosition();

		DebugInfo.Set("cam_zoom", CameraZoom.X);
	}
}
