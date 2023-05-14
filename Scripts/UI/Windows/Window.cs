using System;
using Godot;

namespace Scribble;

public partial class Window : Control
{
    public enum Type
    {
        Full,
        Modal,
        Popup
    }

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


    #region Exported Properties
    [Export] public string KeyName { get; set; }
    [Export] public Type WindowType { get; set; }

    /// <summary>
    /// Can be closed by clicking outside the window
    /// </summary>
    [Export] public bool Dismissible { get; set; } = true;

    [ExportGroup("Visuals")]
    [ExportSubgroup("All Windows")]
    [Export] public Direction SlideInDirection { get; set; }
    /// <summary>
    /// When the fade background is hidden inputs will be passed through the window's control
    /// </summary>
    [Export] public bool ShowFadeBackground { get; set; } = true;
    [Export] public bool ShowContentPanel { get; set; } = true;

    [ExportSubgroup("Only Full Window")]
    [Export] public string Title { get; set; } = "[Untitled_Window]";
    [Export] public bool ShowTitleBar { get; set; } = true;
    #endregion


    WindowTransitions transitions;
    new public event Action Hidden;

    public Panel Panel { get; private set; }
    Control fadeBackground;

    new Vector2 Position => Panel.Position;
    new Vector2 Size => Panel.Size;

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

    bool IsFocusedWindow => GetParent().GetChild<Window>(-1) == this;

    public event Action WindowShow;
    public event Action WindowHide;

    public override void _Ready()
    {
        Panel = GetChild<Panel>(WindowType == Type.Popup ? 0 : 1);
        if (WindowType != Type.Popup)
            fadeBackground = GetChild<Control>(0);


        InitializeTitleBar();
        InitializeContentPanel();


        if (!Dismissible)
            Dismissible = WindowType == Type.Popup || !ShowFadeBackground;

        if (WindowType != Type.Popup)
        {
            if (!ShowFadeBackground)
                fadeBackground.Modulate = Colors.Transparent;

            if (Dismissible)
            {
                fadeBackground.GuiInput += inputEvent =>
                {
                    if (inputEvent is InputEventMouseButton buttonEvent && buttonEvent.Pressed)
                    {
                        Hide();
                        return;
                    }
                };
            }
        }

        Main.WindowSizeChanged += UpdateTargetPosition;

        transitions = new(this);
        transitions.InstaHide();
        transitions.Hidden += () => Hidden?.Invoke();
    }

    public override void _Process(double delta) => transitions.Update((float)delta);

    public override void _Input(InputEvent inputEvent)
    {
        if (!IsFocusedWindow || !Visible)
            return;

        if (Dismissible && inputEvent is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
            Hide();

        if (WindowType == Type.Popup && inputEvent is InputEventMouseButton buttonEvent && buttonEvent.Pressed && !Panel.GetRect().HasPoint(buttonEvent.Position))
            Hide();
    }

    protected void UpdateTargetPosition() => PanelTargetPosition = Main.ViewportRect.GetCenter() - (Size / 2);

    void InitializeTitleBar()
    {
        if (WindowType != Type.Full)
            return;

        Control titleBar = Panel.GetChild(0).GetChild(0).GetChild<Control>(0);
        if (!ShowTitleBar)
        {
            titleBar.Hide();
            return;
        }

        if (Dismissible)
            titleBar.GetChild<Button>(0).Pressed += Hide;
        else
            titleBar.GetChild<Button>(0).Hide();

        titleBar.GetChild<Label>(1).Text = Title;
    }

    void InitializeContentPanel()
    {
        if (ShowContentPanel || WindowType == Type.Modal)
            return;

        if (WindowType == Type.Popup)
        {
            Panel.SelfModulate = Colors.Transparent;
            DisableMargins(Panel.GetChild<MarginContainer>(0));
            return;
        }

        Panel contentPanel = Panel.GetChild(0).GetChild(0).GetChild<Panel>(WindowType == Type.Full ? 1 : 0);
        contentPanel.SelfModulate = Colors.Transparent;

        DisableMargins(contentPanel.GetChild<MarginContainer>(0));
    }

    static void DisableMargins(MarginContainer margins)
    {
        margins.AddThemeConstantOverride("margin_left", 0);
        margins.AddThemeConstantOverride("margin_right", 0);
        margins.AddThemeConstantOverride("margin_top", 0);
        margins.AddThemeConstantOverride("margin_bottom", 0);
    }

    new public Window Show()
    {
        WindowShow?.Invoke();
        GetParent().MoveChild(this, -1); // Move to the bottom of the window stack
        transitions.Show();
        return this;
    }

    new public void Hide()
    {
        WindowHide?.Invoke();
        GetParent().MoveChild(this, 0); // Move to the top of the window stack
        transitions.Hide();
    }
}