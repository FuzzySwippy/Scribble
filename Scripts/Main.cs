using System;
using Godot;

namespace Scribble;

public partial class Main : Node2D
{
	public static Vector2I BaseWindowSize { get; } = new(1920, 1080);

	public static Godot.Window Window { get; private set; }

	public static Rect2 ViewportRect { get; private set; }

	public static Artist Artist { get; private set; }

	public new static event Action Ready;
	public static event Action WindowSizeChanged;

	public override void _Ready()
	{
		Window = GetWindow();
		Window.MinSize = new Vector2I(800, 500);

		Artist = new();

		Ready?.Invoke();
		Window.SizeChanged += WindowSizeChangeHandler;

		WindowSizeChangeHandler();

		Global.Canvas.Init(Temp.CanvasSize, Artist);
	}

	void WindowSizeChangeHandler()
	{
		ViewportRect = GetViewportRect();
		WindowSizeChanged?.Invoke();
	}
}
