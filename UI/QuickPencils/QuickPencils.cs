using System;
using Godot;
using Scribble.Application;
using Scribble.Drawing;

namespace Scribble.UI;

public partial class QuickPencils : Node
{
	private ScribbleLib.ScribbleColor Color => Main.Artist.GetQuickPencilColor(SelectedType);

	private QuickPencilSelector[] selectors = new QuickPencilSelector[4];
	private QuickPencilType selectedType = QuickPencilType.Primary;
	public QuickPencilType SelectedType
	{
		get => selectedType;
		set
		{
			if (selectedType == value)
				return;

			selectedType = value;
			UpdateSelectorVisibility();
			UpdateSelectorColor();
			ColorChanged?.Invoke(Color.GodotColor);
		}
	}

	public event Action<Color> ColorChanged;

	public override void _Ready()
	{
		Global.QuickPencils = this;
		Main.Ready += MainReady;
	}

	private void MainReady()
	{
		GetSelectors();
		SetSelectorBackgroundTextures();
		UpdateSelectorVisibility();
	}

	private void GetSelectors()
	{
		foreach (Node child in GetChildren())
			if (child is QuickPencilSelector selector)
				selectors[(int)selector.Type] = selector;
	}

	private void SetSelectorBackgroundTextures()
	{
		Texture2D texture = TextureGenerator.NewBackgroundTexture(new(7, 7));
		for (int i = 0; i < selectors.Length; i++)
			selectors[i].SetBackground(texture);
	}

	private void UpdateSelectorVisibility()
	{
		for (int i = 0; i < selectors.Length; i++)
			selectors[i].Visible = selectors[i].Type == SelectedType;
	}

	private void UpdateSelectorColor() => selectors[(int)SelectedType].UpdateColor();

	public Color GetColor() => Color.GodotColor;
	public void SetColor(Color color)
	{
		Color.Set(color);
		UpdateSelectorColor();
		ColorChanged?.Invoke(color);
	}
}
