using Godot;
using ScribbleLib;

namespace Scribble;

public partial class PalettePanel : Node
{
    Button editButton;
    OptionButton paletteSelectionDropdown;
    PaletteColorGrid paletteColorGrid;
    Label noPaletteSelectedLabel;

    Palette selectedPalette = null;

    public override void _Ready()
    {
        Global.PalettePanel = this;

        GetControls();
        SetupControls();

        Main.Ready += MainReady;
    }

    void MainReady()
    {
        UpdateSelectionDropdown();
        SelectPalette(-1);
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
        paletteSelectionDropdown.ItemSelected += i => SelectPalette((int)i);
        paletteColorGrid.PaletteUpdated += p => noPaletteSelectedLabel.Visible = p == null;

        paletteColorGrid.Init(Global.MainColorInput, false);
    }

    void SelectPalette(int index)
    {
        if (index == -1)
        {
            selectedPalette = null;
            paletteSelectionDropdown.Select(-1);

            paletteColorGrid.SetPalette(null);
            paletteColorGrid.Hide();

            noPaletteSelectedLabel.Show();
            return;
        }

        selectedPalette = Main.Artist.Palettes[index];
        paletteColorGrid.SetPalette(selectedPalette);

        paletteColorGrid.Show();
        noPaletteSelectedLabel.Hide();

        paletteSelectionDropdown.Select(index);
    }

    public void UpdateSelectionDropdown()
    {
        //Updates palette selection dropdown item names
        paletteSelectionDropdown.Clear();

        for (int i = 0; i < Main.Artist.Palettes.Count; i++)
            paletteSelectionDropdown.AddItem($"{i + 1}. {Main.Artist.Palettes[i].Name}");

        paletteSelectionDropdown.Select(-1);

        //Reselect the selected palette if there is one
        if (selectedPalette == null)
            return;

        SelectPalette(Main.Artist.Palettes.IndexOf(selectedPalette));
    }
}