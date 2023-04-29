using Godot;

namespace Scribble;

public partial class PalettePanel : Node
{
    Button editButton;

    public override void _Ready()
    {
        editButton = GetChild(0).GetChild(0).GetChild(2).GetChild<Button>(1);
        editButton.Pressed += () => WindowManager.Show("palettes");
    }
}
