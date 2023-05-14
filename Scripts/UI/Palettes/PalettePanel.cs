using Godot;
using ScribbleLib;

namespace Scribble;

public partial class PalettePanel : Node
{
    Button editButton;
    OptionButton paletteSelectionDropdown;
    PaletteColorGrid paletteColorGrid;
    Label noPaletteSelectedLabel;

    readonly Palette tempPalette = new("TempPalette", new Color?[Palette.MaxColors]
    {
        new (1, 0, 0, 1),
        new (0, 1, 0, 1),
        new (0, 0, 1, 1),
        null,
        new (1, 1, 0, 0),
        new (1, 0, 1, 1),
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
    });
    Palette Palette => tempPalette;//Main.Artist.CurrentPalette;


    public override void _Ready()
    {
        GetControls();
        SetupControls();
    }

    void GetControls()
    {
        Node parent = this.GetGrandChild(2).GetChild(2);
        paletteSelectionDropdown = parent.GetChild<OptionButton>(0);
        editButton = parent.GetChild<Button>(1);

        parent = this.GetGrandChild(2).GetChild(1);
        paletteColorGrid = parent.GetChild<PaletteColorGrid>(0);
        noPaletteSelectedLabel = parent.GetChild<Label>(1);
    }

    void SetupControls()
    { 
        editButton.Pressed += () => WindowManager.Show("palettes");

        paletteColorGrid.PaletteUpdated += p => noPaletteSelectedLabel.Visible = p == null;

        paletteColorGrid.Init(Global.MainColorInput, false);
        paletteColorGrid.SetPalette(Palette);
    }
}