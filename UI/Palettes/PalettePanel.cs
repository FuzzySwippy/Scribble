using System;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class PalettePanel : Node
{
	private Button editButton;
	private OptionButton paletteSelectionDropdown;
	private PaletteColorGrid paletteColorGrid;
	private Label noPaletteSelectedLabel;
	private Palette selectedPalette = null;

	public override void _Ready()
	{
		Global.PalettePanel = this;

		GetControls();
		SetupControls();

		Main.Ready += MainReady;
	}

	private void MainReady()
	{
		UpdateSelectionDropdown();
		SelectPalette(Main.Artist.Palettes.Count > 0 ? 0 : -1);
	}

	private void GetControls()
	{
		Node parent = this.GetGrandChild(2).GetChild(2);
		paletteSelectionDropdown = parent.GetChild<OptionButton>(0);
		editButton = parent.GetChild<Button>(1);

		parent = this.GetGrandChild(2).GetChild(1);
		paletteColorGrid = parent.GetChild<PaletteColorGrid>(0);
		noPaletteSelectedLabel = parent.GetChild<Label>(1);
	}

	private void SetupControls()
	{
		editButton.Pressed += () => WindowManager.Show("palettes");
		paletteSelectionDropdown.ItemSelected += i => SelectPalette((int)i);
		paletteColorGrid.PaletteUpdated += p => noPaletteSelectedLabel.Visible = p == null;

		paletteColorGrid.Init(null, false);
	}

	private void SelectPalette(int index)
	{
		if (index == -1)
		{
			selectedPalette = null;
			paletteSelectionDropdown.Select(-1);
			paletteSelectionDropdown.Disabled = true;

			paletteColorGrid.SetPalette(null);
			paletteColorGrid.Hide();

			noPaletteSelectedLabel.Show();
			return;
		}

		if (!Main.Artist.Palettes.InRange(index))
			throw new IndexOutOfRangeException();

		selectedPalette = Main.Artist.Palettes[index];
		paletteColorGrid.SetPalette(selectedPalette);

		paletteColorGrid.Show();
		noPaletteSelectedLabel.Hide();

		paletteSelectionDropdown.Disabled = false;
		paletteSelectionDropdown.Select(index);
	}

	public void UpdateSelectionDropdown()
	{
		paletteSelectionDropdown.Disabled = Main.Artist.Palettes.Count == 0;

		//Updates palette selection dropdown item names
		paletteSelectionDropdown.Clear();

		for (int i = 0; i < Main.Artist.Palettes.Count; i++)
			paletteSelectionDropdown.AddItem($"{i + 1}. {Main.Artist.Palettes[i].Name}");

		paletteSelectionDropdown.Select(-1);

		//Reselect the selected palette
		if (selectedPalette != null)
		{
			SelectPalette(Main.Artist.Palettes.IndexOf(selectedPalette));
			return;
		}

		//Select the first palette if there is one
		if (Main.Artist.Palettes.Count > 0)
			SelectPalette(0);
	}
}