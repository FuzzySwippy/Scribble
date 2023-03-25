using System.Collections.Generic;
using Godot;

using Colors = ScribbleLib.Colors;

namespace Scribble;

public partial class ColorController : Node
{
    public List<PencilColorSelector> PencilColorSelectors { get; } = new();

    PencilColorType selectedType = PencilColorType.Primary;
	public PencilColorType SelectedType
	{
        get => selectedType;
        set
        {
			if (selectedType == value)
                return;

            selectedType = value;
            UpdateSelectors();
        }
    }

    public override void _Ready()
    {
        SetSelectorBackgroundTexture();
        UpdateSelectors();
    }

    void SetSelectorBackgroundTexture()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(7,7));
        for (int i = 0; i < PencilColorSelectors.Count; i++)
            PencilColorSelectors[i].SetBackground(texture);
    }

    void UpdateSelectors()
    {
        for (int i = 0; i < PencilColorSelectors.Count; i++)
            PencilColorSelectors[i].Visible = PencilColorSelectors[i].Type == SelectedType;
    }

    public void UpdatePencilColor()
    {
        Main.Artist.Brush.SetPencilColor(SelectedType, Colors.white.Lerp(Global.HueSlider.Color, Global.ColorBox.XValue).Lerp(Colors.black, Global.ColorBox.YValue));
        foreach (PencilColorSelector selector in PencilColorSelectors)
            if (selector.Type == SelectedType)
                selector.UpdateColor();
    }
}
