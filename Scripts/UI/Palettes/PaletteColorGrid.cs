using Godot;
using System;

namespace Scribble;
public partial class PaletteColorGrid : Control
{
	/// <summary>
	/// Then IsEditor is true, the grid won't deselect the current Selector when the color is updated in the ColorInput.
	/// </summary>
	/// <value></value>
	public bool IsEditor { get; private set; }

	bool isInitialized;
	bool isSetup;

	readonly PaletteColorSelector[] selectors = new PaletteColorSelector[Palette.MaxColors];
	ColorInput colorInput;
	public int SelectedColorIndex { get; private set; } = -1;
	bool ignoreColorUpdate;

	Palette palette;

	public event Action<Palette> PaletteUpdated;
	public event Action<int> ColorSelected;

	public override void _Ready() => Main.Ready += MainReady;

	void MainReady()
	{
		GenerateColorSelectors();
		UpdateSelectors();

		isSetup = true;
	}

	void GenerateColorSelectors()
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

	void SelectorRightClicked(InputEvent inputEvent, int index)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed)
		{
			ContextMenu.ShowMenu(mouseEvent.GlobalPosition, new ContextMenuItem[]
			{
				new("Copy hex code", () => DisplayServer.ClipboardSet(palette.GetColor(index).Value.ToHtml())),
				new("Delete", () => DeleteColor(index))
			});
		}
	}

	void ColorUpdated()
	{
		if (ignoreColorUpdate || palette == null)
		{
			ignoreColorUpdate = false;
			return;
		}
		Deselect();
	}

	void EditorColorUpdated()
	{
		if (palette == null || SelectedColorIndex < 0)
			return;

		Color color = colorInput.Color.GodotColor;
		palette.SetColor(color, SelectedColorIndex);
		selectors[SelectedColorIndex].ColorRect.Color = color;
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

		Color? color = palette.GetColor(index);
		if (color == null)
		{
			Deselect();
			return;
		}

		if (!IsEditor)
			ignoreColorUpdate = true;

		SelectedColorIndex = index;
		colorInput.SetColorFromGodotColor(palette.Colors[index].Value);
		UpdateSelectorIndicators();

		ColorSelected?.Invoke(index);
	}

	public void AddColor(int index)
	{
		if (palette == null)
			throw new Exception("Palette is null");

		palette.SetColor(colorInput.Color.GodotColor, index);
		Main.Artist.Palettes.Save();

		UpdateSelectors();
		Select(index);
	}

	public void DeleteColor(int index)
	{
		if (palette == null)
			throw new Exception("Palette is null");

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

	void UpdateSelectorIndicators()
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

	void UpdateSelectors()
	{
		SelectedColorIndex = -1;
		ColorSelected?.Invoke(-1);

		for (int i = 0; i < Palette.MaxColors; i++)
		{
			selectors[i].Hide();
			if (palette == null || !palette.Colors[i].HasValue)
				continue;

			selectors[i].ColorRect.Color = palette.Colors[i].Value;
			selectors[i].Show();
		}
	}
}
