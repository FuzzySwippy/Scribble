using Godot;
using Scribble.Application;

namespace Scribble;

public partial class QuickPencils : Node
{
	private ScribbleLib.ScribbleColor Color => Main.Artist.Brush.GetQuickPencilColor(SelectedType);

	private readonly QuickPencilSelector[] selectors = new QuickPencilSelector[4];
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
			Global.MainColorInput.Color = Color;
			UpdateSelectorColor();
		}
	}

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

		Global.MainColorInput.ColorUpdated += UpdateSelectorColor;
		Global.MainColorInput.Color = Color;
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

	public void UpdateSelectorColor() => selectors[(int)SelectedType].UpdateColor();
}
