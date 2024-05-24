using System;
using Godot;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Application;

public partial class Main : Node2D
{
	public static Vector2I BaseWindowSize { get; } = new(1920, 1080);

	public static Godot.Window Window { get; private set; }

	public static Rect2 ViewportRect { get; private set; }

	public static Artist Artist { get; private set; }

	public static new event Action Ready;
	public static event Action WindowSizeChanged;

	public override void _Ready()
	{
		Global.Main = this;

		Window = GetWindow();
		Window.MinSize = new Vector2I(800, 500);

		Artist = new();

		Ready?.Invoke();
		Window.SizeChanged += WindowSizeChangeHandler;

		WindowSizeChangeHandler();

		Global.Canvas.Init(Canvas.DefaultResolution.ToVector2I(), Artist);
	}

	private void WindowSizeChangeHandler()
	{
		ViewportRect = GetViewportRect();
		WindowSizeChanged?.Invoke();
	}

	public static void Quit() => Global.Main.QuitInternal();
	private void QuitInternal() => GetTree().Quit();
}
