using Godot;

namespace Scribble;

public class Palette
{
    public const int MaxColors = 16;


    public string Name { get; set; }
    public Color?[] Colors { get; set; }

    public Palette(string name)
    {
        Name = name;
        Colors = new Color?[MaxColors];
    }
}
