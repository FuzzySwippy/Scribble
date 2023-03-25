using Godot;

namespace ScribbleLib;

public static class Colors
{
    public static readonly Color white = new(1, 1, 1);
    public static readonly Color black = new(0, 0, 0);
    public static readonly Color red = new(1, 0, 0);
    public static readonly Color yellow = new(1, 1, 0);
    public static readonly Color green = new(0, 1, 0);
    public static readonly Color cyan = new(0, 1, 1);
    public static readonly Color blue = new(0, 0, 1);
    public static readonly Color magenta = new(0, 1, 1);

    public static readonly Color transparent = new(0, 0, 0, 0);
}