using System;
using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ColorInput : Node
{
	ColorBox colorBox;
	HueSlider hueSlider;
	ColorComponentSlider rComponent;
	ColorComponentSlider gComponent;
	ColorComponentSlider bComponent;
	ColorComponentSlider aComponent;
	HexInput hexInput;

    readonly ScribbleColor color = new();
    public ScribbleColor Color
    {
        get => color;
        set
        {
            color.CloneFrom(value);
            ColorUpdated?.Invoke();
            UpdateVisualizations();
        }
    }

    public event Action ColorUpdated;

    public override void _Ready()
    {
        SetupValueSelectors();
        SetColorComponentBackgroundTextures();

        Main.Ready += UpdateVisualizations;
    }

    void SetupValueSelectors()
    {
        Node container = GetChild(0).GetChild(0);

        colorBox = container.GetChild(0).GetChild<ColorBox>(0);
        hueSlider = container.GetChild(0).GetChild<HueSlider>(1);
        rComponent = container.GetChild<ColorComponentSlider>(1);
        gComponent = container.GetChild<ColorComponentSlider>(2);
        bComponent = container.GetChild<ColorComponentSlider>(3);
        aComponent = container.GetChild<ColorComponentSlider>(4);
        hexInput = container.GetChild<HexInput>(5);

        colorBox.Parent = this;
        hueSlider.Parent = this;
        rComponent.Parent = this;
        gComponent.Parent = this;
        bComponent.Parent = this;
        aComponent.Parent = this;
        hexInput.Parent = this;
    }

    void SetColorComponentBackgroundTextures()
    {
        Texture2D texture = TextureGenerator.NewBackgroundTexture(new(28, 3));
        rComponent.SetBackground(texture);
		gComponent.SetBackground(texture);
		bComponent.SetBackground(texture);
        aComponent.SetBackground(texture);
    }
    public void SetColorFromHueAndColorBox()
    {
        Color.SetHSVA(hueSlider.HValue, colorBox.SValue, colorBox.VValue, aComponent.Value);
        ColorUpdated?.Invoke();
        GD.Print($"CD");
        UpdateVisualizations();
    }

    public void SetColorFromComponentSliders()
    {
        Color.SetRGBA(rComponent.Value, gComponent.Value, bComponent.Value, aComponent.Value);
        ColorUpdated?.Invoke();
        GD.Print($"CD2");
        UpdateVisualizations();
    }

    public void SetColorFromHexInput()
    {
        if (!hexInput.Color.HasValue)
            return;

        Color.SetFromRGBA(hexInput.Color.Value);
        ColorUpdated?.Invoke();
        GD.Print($"CD32");
        UpdateVisualizations();
    }

    public void UpdateVisualizations()
    {
        colorBox.UpdateVisualization();
        hueSlider.UpdateVisualization();
        hexInput.UpdateVisualizations();

        rComponent.Value = Color.R;
        gComponent.Value = Color.G;
        bComponent.Value = Color.B;
        aComponent.Value = Color.A;
    }
}
