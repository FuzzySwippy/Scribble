using Godot;

namespace Scribble;

public class Palette
{
    public const int MaxColors = 16;


    public string Name { get; set; }
    public Color?[] Colors { get; } = new Color?[MaxColors];

    public Palette(string name, Color?[] colors)
    {
        if (colors.Length > MaxColors)
            throw new System.ArgumentOutOfRangeException(nameof(colors), $"Palette can only have {MaxColors} colors.");

        Name = name;
        colors?.CopyTo(Colors, 0);
    }

    public Palette(string name) : this(name, null) { }

    public bool GetColor(int index, out Color? color)
    {
        if (index < 0 || index >= Colors.Length)
            throw new System.ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Colors.Length - 1}.");

        color = Colors[index];
        return color.HasValue;
    }
    
    public bool SetColor(Color? color, int index)
    {
        if (index < 0 || index >= Colors.Length)
            throw new System.ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Colors.Length - 1}.");

        if (Colors[index] == color)
            return false;

        Colors[index] = color;
        return true;
    }
}
