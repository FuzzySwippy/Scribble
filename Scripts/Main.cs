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

        //Later create a new artist when new canvas settings have been chosen
        Artist = new(Temp.CanvasSize);

        Ready?.Invoke();
        Window.SizeChanged += WindowSizeChangeHandler;

        WindowSizeChangeHandler();
    }

    void WindowSizeChangeHandler()
    {
        ViewportRect = GetViewportRect();
        WindowSizeChanged?.Invoke();
    }

    public override void _Process(double delta) => Artist?.Update();

    /// <summary>
    /// Stops further propagation of the input event.
    /// </summary>
    public static void InputEventHandled() => Window.SetInputAsHandled();
}
