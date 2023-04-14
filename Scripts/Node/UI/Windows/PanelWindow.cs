using Godot;

namespace Scribble;

public partial class PanelWindow : Control
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

    [Export] public new string Name { get; set; }
    [Export] public Direction OriginDirection { get; set; }

    public Vector2 OriginPosition
    {
        get => OriginDirection switch
        {
            Direction.Left => new(-Size.X - WindowVisualizer.OriginMargin, Position.Y),
            Direction.Right => new(Main.ViewportRect.Size.X + WindowVisualizer.OriginMargin, Position.Y),
            Direction.Top => new(Position.X, -Size.Y - WindowVisualizer.OriginMargin),
            Direction.Bottom => new(Position.X, Main.ViewportRect.Size.Y + WindowVisualizer.OriginMargin),
            Direction.TopLeft => new(-Size.X - WindowVisualizer.OriginMargin, -Size.Y - WindowVisualizer.OriginMargin),
            Direction.TopRight => new(Main.ViewportRect.Size.X + WindowVisualizer.OriginMargin, -Size.Y - WindowVisualizer.OriginMargin),
            Direction.BottomLeft => new(-Size.X - WindowVisualizer.OriginMargin, Main.ViewportRect.Size.Y + WindowVisualizer.OriginMargin),
            Direction.BottomRight => new(Main.ViewportRect.Size.X + WindowVisualizer.OriginMargin, Main.ViewportRect.Size.Y + WindowVisualizer.OriginMargin),
            _ => new(),
        };
    }

    public Vector2 TargetPosition { get; private set; }

    public override void _Ready()
    {
        GetChild(0).GetChild(0).GetChild(0).GetChild<Button>(0).Pressed += () => WindowManager.Hide(this);
        GetChild(0).GetChild(0).GetChild(0).GetChild<Label>(1).Text = Name;

        Main.WindowSizeChanged += () => TargetPosition = Main.ViewportRect.GetCenter() - (Size / 2);
    }
}
