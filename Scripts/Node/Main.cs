using Godot;

namespace Scribble;

public partial class Main : Node2D
{
    public static Vector2I BaseWindowSize { get; } = new(1920, 1080);
    public static Window Window { get; private set; }

    public Artist Artist { get; set; }

    public override void _Ready()
    {
        Window = GetWindow();
        Window.MinSize = new Vector2I(800, 500);

        //Later create a new artist when new canvas settings have been chosen
        Artist = new(Temp.CanvasSize);
    }

    public override void _Process(double delta) => Artist?.Update();
}
