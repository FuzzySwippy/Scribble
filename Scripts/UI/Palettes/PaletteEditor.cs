using Godot;
using ScribbleLib;
using System.Linq;

namespace Scribble;

public partial class PaletteEditor : Node
{
	ItemList paletteList;
    Control noPalettesControl;
	ColorInput colorInput;

	//New palette
	LineEdit newPaletteNameInput;
	Button addPaletteButton;


	//Current palette
	LineEdit selectedPaletteNameInput;
	Button deletePaletteButton;
	Control selectedPaletteControl;
	Control noPaletteSelectedControl;


	int selectedColorIndex = -1;
	int selectedPaletteIndex = -1;
	Palette SelectedPalette => Main.Artist.Palettes.Get(selectedPaletteIndex);

	public override void _Ready()
	{
        GetControls();
		SetupControls();

		Main.Ready += MainReady;
    }

    void MainReady()
    { 
        UpdatePaletteList();
	}

    void GetControls()
	{
        paletteList = this.GetGrandChild<ItemList>(6);
		noPalettesControl = this.GetGrandChild(4).GetChild<Control>(1);
        colorInput = GetChild(0).GetChild(1).GetChild<ColorInput>(0);

		//New palette
		Node parent = this.GetGrandChild(2).GetChild(1).GetGrandChild(2);
        newPaletteNameInput = parent.GetChild<LineEdit>(0);
		addPaletteButton = parent.GetChild<Button>(1);

		//Current palette
        parent = GetChild(0).GetGrandChild(2, 1).GetChild(0);
		noPaletteSelectedControl = parent.GetChild<Control>(1);
		selectedPaletteControl = parent.GetChild<Control>(0);

		parent = parent.GetGrandChild(2);
        selectedPaletteNameInput = parent.GetChild<LineEdit>(0);
		deletePaletteButton = parent.GetChild(2).GetChild<Button>(0);
	}

    void SetupControls()
    { 
		addPaletteButton.Pressed += CreatePalette;
		deletePaletteButton.Pressed += DeleteSelectedPalette;
	}

    void UpdatePaletteList()
    {
		DeselectPalette();
        paletteList.Clear();

        bool hasPalettes = Main.Artist.Palettes.Count > 0;
        noPalettesControl.Visible = !hasPalettes;
        paletteList.Visible = hasPalettes;

        if (!hasPalettes)
            return;

        for (int i = 0; i < Main.Artist.Palettes.Count; i++)
            paletteList.AddItem($"{i + 1}. {Main.Artist.Palettes[i].Name}");
    }

    void SelectPalette(int index)
    { 
		selectedPaletteIndex = index;
		selectedColorIndex = -1;
		colorInput.Interactable = false;
		
		bool hasPalette = SelectedPalette != null;
		selectedPaletteControl.Visible = hasPalette;
        noPaletteSelectedControl.Visible = !hasPalette;

        /*if (selectedPalette == null)
		{
			
			selectedPaletteNameInput.Text = "";
			deletePaletteButton.Disabled = true;
			return;
		}*/
    }

	void DeselectPalette() => SelectPalette(-1);

    void CreatePalette()
    { 
		string name = newPaletteNameInput.Text;
        if (string.IsNullOrWhiteSpace(name))
        {
			WindowManager.ShowModalOk("Palette name cannot be empty.");
            return;
        }

        if (Main.Artist.Palettes.Any(p => p.Name == name))
        { 
			WindowManager.ShowModalOk($"Palette with name '{name}' already exists.");
            return;
        }

		newPaletteNameInput.Text = "";
		newPaletteNameInput.ReleaseFocus();

		Main.Artist.Palettes.Add(new(name));
		UpdatePaletteList();
    }

    void DeleteSelectedPalette()
    { 
		if (SelectedPalette == null)
			return;
	}
}
