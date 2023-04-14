using Godot;

namespace Scribble;

public class Palette
{
    public string Name { get; set; }
    public Color[] Colors { get; set; }

    public Palette(string name)
    {
        Name = name;
        Colors = new Color[18]; //Might set to 16 in the future
    }
}
