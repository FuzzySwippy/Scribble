using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib;
using System;

namespace Scribble.UI;
public partial class PaletteColorGrid : Control
{
	/// <summary>
	/// Then IsEditor is true, the grid won't deselect the current Selector when the color is updated in the ColorInput.
	/// </summary>
	/// <value></value>
	public bool IsEditor { get; private set; }

	private bool isInitialized;
	private bool isSetup;
	private readonly PaletteColorSelector[] selectors = new PaletteColorSelector[Palette.MaxColors];
	private Control lockedIndicator;
	private ColorInput colorInput;
	public int SelectedColorIndex { get; private set; } = -1;

	private bool ignoreColorUpdate;
	private Palette palette;

	public event Action<Palette> PaletteUpdated;
	public event Action<int> ColorSelected;

	public override void _Ready() => Main.Ready += MainReady;

	private void MainReady()
	{
		lockedIndicator = GetChild<Control>(1);
		lockedIndicator.Hide();

		GenerateColorSelectors();
		UpdateSelectors();

		isSetup = true;
	}

	private void GenerateColorSelectors()
	{
		Texture2D backgroundTexture = TextureGenerator.NewBackgroundTexture(new(5, 5));
		Node selectorParent = GetChild(0);
		Control baseColorSelector = selectorParent.GetChild<Control>(0);

		for (int i = 0; i < Palette.MaxColors; i++)
		{
			if (i == 0)
				selectors[i] = new(baseColorSelector);
			else
			{
				selectors[i] = new((Control)baseColorSelector.Duplicate());
				selectorParent.AddChild(selectors[i].Control);
			}

			int index = i;
			selectors[i].ColorButton.Pressed += () => Select(index);
			selectors[i].ColorButton.GuiInput += e => SelectorRightClicked(e, index);
			selectors[i].AddButton.Pressed += () => AddColor(index);
			selectors[i].ColorButton.GetChild<TextureRect>(0).Texture = backgroundTexture;
		}
	}

	private void SelectorRightClicked(InputEvent inputEvent, int index)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed)
		{
			ContextMenu.ShowMenu(mouseEvent.GlobalPosition, new ContextMenuItem[]
			{
				new("Copy hex code", () => DisplayServer.ClipboardSet(palette.GetColor(index).HexCode)),
				palette.Locked ? null : new("Delete", () => DeleteColor(index))
			});
		}
	}

	private void ColorUpdated()
	{
		if (ignoreColorUpdate || palette == null)
		{
			ignoreColorUpdate = false;
			return;
		}
		Deselect();
	}

	private void EditorColorUpdated()
	{
		if (palette == null || SelectedColorIndex < 0 || palette.Locked)
			return;

		SimpleColor color = colorInput.Color.SimpleColor;
		palette.SetColor(color, SelectedColorIndex);
		selectors[SelectedColorIndex].ColorRect.Color = color.GodotColor;
		Main.Artist.Palettes.MarkForSave();
	}

	public void Init(ColorInput newColorInput, bool isEditor)
	{
		if (isInitialized)
			throw new Exception("PaletteColorGrid already initialized");

		IsEditor = isEditor;

		colorInput = newColorInput ?? throw new Exception("ColorInput is null");
		colorInput.ColorUpdated += IsEditor ? EditorColorUpdated : ColorUpdated;

		isInitialized = true;
	}

	public void SetPalette(Palette newPalette)
	{
		palette = newPalette;
		PaletteUpdated?.Invoke(palette);

		if (isInitialized && isSetup)
			UpdateSelectors();
	}

	public void Select(int index)
	{
		if (palette == null)
			throw new Exception("Palette is null");

		if (index < 0)
		{
			Deselect();
			return;
		}

		SimpleColor color = palette.GetColor(index);
		if (color == null)
		{
			Deselect();
			return;
		}

		if (!IsEditor)
			ignoreColorUpdate = true;

		SelectedColorIndex = index;
		colorInput.SetColor(palette[index]);
		UpdateSelectorIndicators();

		ColorSelected?.Invoke(index);
	}

	public void AddColor(int index)
	{
		if (palette == null)
			throw new Exception("Palette is null");

		if (palette.Locked)
			return;

		palette.SetColor(colorInput.Color.SimpleColor, index);
		Main.Artist.Palettes.Save();

		UpdateSelectors();
		Select(index);
	}

	public void DeleteColor(int index)
	{
		if (palette == null)
			throw new Exception("Palette is null");

		if (palette.Locked)
			return;

		palette.SetColor(null, index);
		Main.Artist.Palettes.Save();

		UpdateSelectors();
	}

	public void Deselect()
	{
		if (palette == null)
			throw new Exception("Palette is null");

		SelectedColorIndex = -1;
		UpdateSelectorIndicators();

		ColorSelected?.Invoke(-1);
	}

	private void UpdateSelectorIndicators()
	{
		if (palette == null)
			throw new Exception("Palette is null");

		for (int i = 0; i < Palette.MaxColors; i++)
		{
			if (i == SelectedColorIndex)
			{
				selectors[i].SelectionIndicator.Show();
				continue;
			}
			selectors[i].SelectionIndicator.Hide();
		}
	}

	private void UpdateSelectors()
	{
		lockedIndicator.Visible = !IsEditor && (palette?.Locked ?? false);

		SelectedColorIndex = -1;
		ColorSelected?.Invoke(-1);

		for (int i = 0; i < Palette.MaxColors; i++)
		{
			selectors[i].Hide();
			if (palette == null)
				continue;

			if (palette[i] == null)
			{
				selectors[i].UpdateAddButton(palette.Locked);
				continue;
			}

			selectors[i].ColorRect.Color = palette[i].GodotColor;
			selectors[i].Show();
		}
	}
}
