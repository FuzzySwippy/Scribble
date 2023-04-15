using System.Collections.Generic;
using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorController : Node
{
    public List<PencilTypeSelector> PencilTypeSelectors { get; } = new();
    public List<ColorComponentSlider> ColorComponentSliders { get; } = new();

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
            UpdateVisualizations();
        }
    }

    public override void _Ready()
    {
        SetSelectorBackgroundTextures();
        SetColorComponentBackgroundTextures();
        UpdateSelectors();

        Main.Ready += UpdateVisualizations;
    }

    void SetSelectorBackgroundTextures()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(7, 7));
        for (int i = 0; i < PencilTypeSelectors.Count; i++)
            PencilTypeSelectors[i].SetBackground(texture);
    }

    void SetColorComponentBackgroundTextures()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(28, 3));
        for (int i = 0; i < ColorComponentSliders.Count; i++)
            ColorComponentSliders[i].SetBackground(texture);
    }

    void UpdateSelectors()
    {
        for (int i = 0; i < PencilTypeSelectors.Count; i++)
            PencilTypeSelectors[i].Visible = PencilTypeSelectors[i].Type == SelectedType;
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

    public void SetColorFromHexInput()
    {
        if (!Global.HexInput.Color.HasValue)
            return;

        Color.Set(Global.HexInput.Color.Value);
        UpdateVisualizations();
    }

    void UpdateVisualizations()
    {
        UpdateInputVisualizations();
        UpdateRGBAVisualization();
        UpdatePencilSelectorColor();
    }

    public void UpdateInputVisualizations()
    {
        Global.ColorBox.UpdateVisualization();
        Global.HueSlider.UpdateVisualization();
        Global.HexInput.UpdateVisualizations();
    }

    public void UpdateRGBAVisualization()
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
