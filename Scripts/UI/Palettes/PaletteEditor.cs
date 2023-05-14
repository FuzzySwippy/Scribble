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
	PaletteColorGrid paletteColorGrid;
	Button deletePaletteButton;
	Control selectedPaletteControl;
	Control noPaletteSelectedControl;

	int selectedPaletteIndex = -1;
	Palette SelectedPalette => Main.Artist.Palettes.Get(selectedPaletteIndex);

	#region Setup
	public override void _Ready()
	{
        GetControls();
		SetupControls();

		Main.Ready += MainReady;
    }

    void MainReady() => UpdatePaletteList();

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
		paletteColorGrid = parent.GetChild<PaletteColorGrid>(1);
		deletePaletteButton = parent.GetChild(2).GetChild<Button>(0);
	}

    void SetupControls()
    { 
		//New palette creation
		addPaletteButton.Pressed += CreatePalette;

		//Palette selection from the list
		paletteList.ItemSelected += PaletteListItemSelected;

        //Palette selection item right-click handling
        paletteList.ItemClicked += PaletteListItemRightClicked;

		//Changing the palette name
		selectedPaletteNameInput.TextSubmitted += UpdateSelectedPaletteName;

		//Deleting the currently selected palette
		deletePaletteButton.Pressed += DeleteSelectedPalette;

		//Initializing the palette color grid
        paletteColorGrid.Init(colorInput, true);
		paletteColorGrid.ColorSelected += PaletteColorSelected;
    }
    #endregion

    #region Events
    void PaletteColorSelected(int index) => colorInput.Interactable = index >= 0;

    void PaletteListItemSelected(long index) => SelectPalette((int)index);

    void PaletteListItemRightClicked(long index, Vector2 position, long mouseButtonIndex)
    {
        if (index != selectedPaletteIndex)
            return;

        if (mouseButtonIndex == (int)MouseButton.Right)
            ContextMenu.ShowMenu(paletteList.GlobalPosition + position, new ContextMenuItem("Delete", DeleteSelectedPalette));
    }
	#endregion

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
		colorInput.Interactable = false;
		
		bool hasPalette = SelectedPalette != null;
		selectedPaletteControl.Visible = hasPalette;
        noPaletteSelectedControl.Visible = !hasPalette;

        if (hasPalette)
        {
			//Doesn't trigger the selected event
            paletteList.Select(index);

			selectedPaletteNameInput.Text = SelectedPalette.Name;
			paletteColorGrid.SetPalette(SelectedPalette);
		}
		else
			paletteColorGrid.SetPalette(null);
    }

	void DeselectPalette() => SelectPalette(-1);

    void CreatePalette()
    { 
		string name = newPaletteNameInput.Text;
        if (string.IsNullOrWhiteSpace(name))
        {
			WindowManager.ShowModal("Palette name cannot be empty.", ModalOptions.Ok);
            return;
        }

        if (Main.Artist.Palettes.Any(p => p.Name == name))
        { 
			WindowManager.ShowModal($"Palette with name '{name}' already exists.", ModalOptions.Ok);
            return;
        }

		newPaletteNameInput.Text = "";
		newPaletteNameInput.ReleaseFocus();

		Main.Artist.Palettes.Insert(0, new(name));
		UpdatePaletteList();
		SelectPalette(0);
        ((ScrollContainer)paletteList.GetParent()).ScrollVertical = 0;
    }

    void DeleteSelectedPalette()
    { 
		if (SelectedPalette == null)
			return;

		WindowManager.ShowModal($"Are you sure you want to delete palette '{SelectedPalette.Name}'?", ModalOptions.YesNo, () => 
		{
            Main.Artist.Palettes.RemoveAt(selectedPaletteIndex);
            UpdatePaletteList();
		});
	}

	void UpdateSelectedPaletteName(string newName)
	{
		if (SelectedPalette == null)
			return;

		if (string.IsNullOrWhiteSpace(newName))
		{
			WindowManager.ShowModal("Palette name cannot be empty.", ModalOptions.Ok);
			return;
		}

		if (Main.Artist.Palettes.Any(p => p.Name == newName))
		{
			WindowManager.ShowModal($"Palette with name '{newName}' already exists.", ModalOptions.Ok);
			return;
		}

        int paletteIndex = selectedPaletteIndex;
        SelectedPalette.Name = newName;
		UpdatePaletteList();
		SelectPalette(paletteIndex);
	}
}
