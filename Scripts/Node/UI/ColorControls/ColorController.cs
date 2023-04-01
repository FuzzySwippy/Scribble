using System.Collections.Generic;
using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorController : Node
{
    public List<PencilTypeSelector> PencilTypeSelectors { get; } = new();

    public ScribbleColor Color => Main.Artist.Brush.PencilColor(SelectedType);

    PencilType selectedType = PencilType.Primary;
	public PencilType SelectedType
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

        Main.Ready += MainReady;
    }

    void MainReady()
    {
        UpdateHueAndColorBoxVisualization();
        UpdateColorComponentVisualization();
    }

    void SetSelectorBackgroundTexture()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(7,7));
        for (int i = 0; i < PencilTypeSelectors.Count; i++)
            PencilTypeSelectors[i].SetBackground(texture);
    }

    void UpdateSelectors()
    {
        for (int i = 0; i < PencilTypeSelectors.Count; i++)
            PencilTypeSelectors[i].Visible = PencilTypeSelectors[i].Type == SelectedType;
    }

    void UpdateVisualizations()
    {
        UpdateHueAndColorBoxVisualization();
        UpdateColorComponentVisualization();
        UpdatePencilSelectorColor();
    }

    public void SetColorFromHueAndColorBox()
    {
        Color.SetHSVA(Global.HueSlider.HValue, Global.ColorBox.SValue, Global.ColorBox.VValue, Global.AComponent.Value);
        UpdateVisualizations();
    }
    public void SetColorFromComponentSliders()
    {
        Color.SetRGBA(Global.RComponent.Value, Global.GComponent.Value, Global.BComponent.Value, Global.AComponent.Value);
        UpdateVisualizations();
    }


    public void UpdateHueAndColorBoxVisualization()
    {
        Global.ColorBox.UpdateVisualization();
        Global.HueSlider.UpdateVisualization();
    }

    public void UpdateColorComponentVisualization()
    {
        Global.RComponent.Value = Color.R;
        Global.GComponent.Value = Color.G;
        Global.BComponent.Value = Color.B;
        Global.AComponent.Value = Color.A;
    }

    public void UpdatePencilSelectorColor()
    {
        foreach (PencilTypeSelector selector in PencilTypeSelectors)
            if (selector.Type == SelectedType)
                selector.UpdateColor();
    }
}
