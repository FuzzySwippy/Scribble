using Godot;

namespace Scribble;

public partial class Window : Control
{
    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public static float PanelStartMargin { get; } = 100;


    [Export] public string KeyName { get; set; }
    [Export] public string DisplayName { get; set; }

    [Export] public Direction SlideInDirection { get; set; }
    WindowTransitions transitions;

    Control panel;
    new Vector2 Position => panel.Position;
    new Vector2 Size => panel.Size;

    public Vector2 PanelStartPosition => SlideInDirection switch
    {
        Direction.Left => new(-Size.X - PanelStartMargin, Position.Y),
        Direction.Right => new(Main.ViewportRect.Size.X + PanelStartMargin, Position.Y),
        Direction.Top => new(Position.X, -Size.Y - PanelStartMargin),
        Direction.Bottom => new(Position.X, Main.ViewportRect.Size.Y + PanelStartMargin),
        Direction.TopLeft => new(-Size.X - PanelStartMargin, -Size.Y - PanelStartMargin),
        Direction.TopRight => new(Main.ViewportRect.Size.X + PanelStartMargin, -Size.Y - PanelStartMargin),
        Direction.BottomLeft => new(-Size.X - PanelStartMargin, Main.ViewportRect.Size.Y + PanelStartMargin),
        Direction.BottomRight => new(Main.ViewportRect.Size.X + PanelStartMargin, Main.ViewportRect.Size.Y + PanelStartMargin),
        _ => new(),
    };
    public Vector2 PanelTargetPosition { get; private set; }

    public override void _Ready()
    {
        panel = GetChild<Control>(1);

        Node titleBar = GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        titleBar.GetChild<Button>(0).Pressed += Hide;
        titleBar.GetChild<Label>(1).Text = DisplayName;

        GuiInput += GuiInputEvent;

        Main.WindowSizeChanged += () => PanelTargetPosition = Main.ViewportRect.GetCenter() - (Size / 2);

        GD.Print(Size);
        transitions = new(this);
        transitions.InstaHide();
    }

    public override void _Process(double delta) => transitions.Update((float)delta);

    void GuiInputEvent(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
            Hide();
    }

    new public Window Show() => transitions.Show();
    new public void Hide() => transitions.Hide();
}