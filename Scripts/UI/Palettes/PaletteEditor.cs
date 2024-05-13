using Godot;
using ScribbleLib;

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
	Control selectedPaletteControl;
	Control noPaletteSelectedControl;

	//Palette buttons
	Button deletePaletteButton;
	Button duplicatePaletteButton;
	Button lockPaletteButton;
	Button unlockPaletteButton;


	int selectedPaletteIndex = -1;
	Palette SelectedPalette => Main.Artist.Palettes[selectedPaletteIndex];

	#region Setup
	public override void _Ready()
	{
		GetControls();
		SetupControls();

		Main.Ready += MainReady;
	}

	void MainReady()
	{
		UpdatePaletteList();
		WindowManager.Get("palettes").WindowHide += WindowHide;
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
		paletteColorGrid = parent.GetChild<PaletteColorGrid>(1);

		//Palette buttons
		deletePaletteButton = parent.GetChild(2).GetChild<Button>(0);
		duplicatePaletteButton = parent.GetChild(2).GetChild<Button>(3);
		lockPaletteButton = parent.GetChild(2).GetChild<Button>(1);
		unlockPaletteButton = parent.GetChild(2).GetChild<Button>(2);
	}

	void SetupControls()
	{
		//New palette creation
		addPaletteButton.Pressed += () => CreatePalette(false);

		//Palette selection from the list
		paletteList.ItemSelected += PaletteListItemSelected;

		//Palette selection item right-click handling
		paletteList.ItemClicked += PaletteListItemRightClicked;

		//Changing the palette name
		selectedPaletteNameInput.TextSubmitted += PaletteNameChanged;

		//Selected palette buttons
		deletePaletteButton.Pressed += DeleteSelectedPalette;
		duplicatePaletteButton.Pressed += () => CreatePalette(true);
		lockPaletteButton.Pressed += ToggleSelectedPaletteLock;
		unlockPaletteButton.Pressed += ToggleSelectedPaletteLock;

		//Initializing the palette color grid
		paletteColorGrid.Init(colorInput, true);
		paletteColorGrid.ColorSelected += PaletteColorSelected;
	}
	#endregion

	#region Events
	void PaletteColorSelected(int index) => colorInput.Interactable = index >= 0 && !SelectedPalette.Locked;

	void PaletteListItemSelected(long index) => SelectPalette((int)index);

	void PaletteListItemRightClicked(long index, Vector2 position, long mouseButtonIndex)
	{
		if (index != selectedPaletteIndex || SelectedPalette != null && SelectedPalette.Locked)
			return;

		if (mouseButtonIndex == (int)MouseButton.Right)
			ContextMenu.ShowMenu(paletteList.GlobalPosition + position, new ContextMenuItem("Delete", DeleteSelectedPalette));
	}

	void WindowHide()
	{
		if (Main.Artist.Palettes.MarkedForSave)
			Main.Artist.Palettes.Save();

		Global.PalettePanel.UpdateSelectionDropdown();
	}
	#endregion

	void UpdatePaletteList(bool reselect = false)
	{
		Palette palette = SelectedPalette;

		DeselectPalette();
		paletteList.Clear();

		bool hasPalettes = Main.Artist.Palettes.Count > 0;
		noPalettesControl.Visible = !hasPalettes;
		paletteList.Visible = hasPalettes;

		if (!hasPalettes)
			return;

		for (int i = 0; i < Main.Artist.Palettes.Count; i++)
			paletteList.AddItem($"{i + 1}. {Main.Artist.Palettes[i].Name}", Main.Artist.Palettes[i].Locked ? Global.LockIconTexture : null);

		if (reselect && palette != null)
			SelectPalette(Main.Artist.Palettes.IndexOf(palette));
	}

	void UpdatePaletteLockControls()
	{
		if (SelectedPalette == null)
			return;

		bool isLocked = SelectedPalette.Locked;

		lockPaletteButton.Visible = !isLocked;
		unlockPaletteButton.Visible = isLocked;
		deletePaletteButton.Visible = !isLocked;

		selectedPaletteNameInput.Editable = !isLocked;
		colorInput.Interactable = !isLocked;
	}

	void SelectPalette(int index)
	{
		if (Main.Artist.Palettes.MarkedForSave)
			Main.Artist.Palettes.Save();

		selectedPaletteIndex = index;
		colorInput.Interactable = false;

		bool hasPalette = SelectedPalette != null;
		selectedPaletteControl.Visible = hasPalette;
		noPaletteSelectedControl.Visible = !hasPalette;

		selectedPaletteNameInput.ReleaseFocus();

		if (!hasPalette)
		{
			paletteColorGrid.SetPalette(null);
			return;
		}

		//Doesn't trigger the selected event
		paletteList.Select(index);

		selectedPaletteNameInput.Text = SelectedPalette.Name;
		paletteColorGrid.SetPalette(SelectedPalette);
		UpdatePaletteLockControls();
	}

	void DeselectPalette() => SelectPalette(-1);

	void CreatePalette(bool duplicateSelected)
	{
		Palette newPalette;
		if (duplicateSelected)
		{
			if (SelectedPalette == null)
				return;

			newPalette = SelectedPalette.Duplicate();
			newPalette.Locked = false;
		}
		else
		{
			string name = newPaletteNameInput.Text;
			if (string.IsNullOrWhiteSpace(name))
			{
				WindowManager.ShowModal("Palette name cannot be empty.", ModalOptions.Ok);
				return;
			}

			newPaletteNameInput.Text = "";
			newPaletteNameInput.ReleaseFocus();
			newPalette = new(name);
		}

		Main.Artist.Palettes.Add(newPalette);
		UpdatePaletteList();
		SelectPalette(0);
		((ScrollContainer)paletteList.GetParent()).ScrollVertical = 0;
	}

	void DeleteSelectedPalette()
	{
		if (SelectedPalette == null || SelectedPalette.Locked)
			return;

		WindowManager.ShowModal($"Are you sure you want to delete palette '{SelectedPalette.Name}'?", ModalOptions.YesNo, () =>
		{
			Main.Artist.Palettes.RemoveAt(selectedPaletteIndex);
			UpdatePaletteList();
		});
	}

	void ToggleSelectedPaletteLock()
	{
		if (SelectedPalette == null)
			return;

		if (SelectedPalette.Locked)
		{
			WindowManager.ShowModal($"Are you sure you want to unlock palette '{SelectedPalette.Name}'?", ModalOptions.YesNo, () =>
			{
				SelectedPalette.Locked = false;
				UpdatePaletteList(true);
			});
		}
		else
		{
			SelectedPalette.Locked = true;
			UpdatePaletteList(true);
		}
	}

	void PaletteNameChanged(string newName)
	{
		if (SelectedPalette == null || SelectedPalette.Locked)
			return;

		if (string.IsNullOrWhiteSpace(newName))
		{
			WindowManager.ShowModal("Palette name cannot be empty.", ModalOptions.Ok);
			return;
		}

		int paletteIndex = selectedPaletteIndex;
		SelectedPalette.Name = newName;

		int selectedColorIndex = paletteColorGrid.SelectedColorIndex;
		UpdatePaletteList();
		SelectPalette(paletteIndex);

		if (selectedColorIndex >= 0)
			paletteColorGrid.Select(selectedColorIndex);
	}
}
