using Godot;
using System.Collections.Generic;
using System.Globalization;

namespace ScribbleLib;

public enum ColorComponent
{
    R,
    G,
    B,
    A
}

public static class ScribbleColors
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

    public static Dictionary<ColorComponent, Color> ComponentMap { get; } = new()
    {
        { ColorComponent.R, red },
        { ColorComponent.G, green },
        { ColorComponent.B, blue },
        { ColorComponent.A, black },
    };


    public static Color SetA(this Color color, float a) => new(color.R, color.G, color.B, a);
}