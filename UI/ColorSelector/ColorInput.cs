using System;
using Godot;
using Scribble.ScribbleLib;

namespace Scribble.UI;

public partial class ColorInput : Node
{
	private ColorBox colorBox;
	private HueSlider hueSlider;
	private ColorComponentSlider rComponent;
	private ColorComponentSlider gComponent;
	private ColorComponentSlider bComponent;
	private ColorComponentSlider aComponent;
	private HexInput hexInput;
	private Control interactionBlocker;
	private ScribbleColor color = new();
	public ScribbleColor Color
	{
		get => color;
		set
		{
			color = value;
			UpdateVisualizations();
		}
	}

	public event Action ColorUpdated;

	public bool Interactable
	{
		get => !interactionBlocker.Visible;
		set => interactionBlocker.Visible = !value;
	}

	public override void _Ready()
	{
		interactionBlocker = GetChild<Control>(1);
		Interactable = true;

		SetupValueSelectors();
		UpdateVisualizations();
	}

	private void SetupValueSelectors()
	{
		Node container = GetChild(0).GetChild(0);

		colorBox = container.GetChild(0).GetChild<ColorBox>(0);
		hueSlider = container.GetChild(0).GetChild<HueSlider>(1);
		rComponent = container.GetChild<ColorComponentSlider>(1);
		gComponent = container.GetChild<ColorComponentSlider>(2);
		bComponent = container.GetChild<ColorComponentSlider>(3);
		aComponent = container.GetChild<ColorComponentSlider>(4);
		hexInput = container.GetChild<HexInput>(5);

		colorBox.ColorInput = this;
		hueSlider.Parent = this;
		rComponent.ColorInput = this;
		gComponent.ColorInput = this;
		bComponent.ColorInput = this;
		aComponent.ColorInput = this;
		hexInput.ColorInput = this;
	}

	public void Set(Color color)
	{
		Color.Set(color);
		ColorUpdated?.Invoke();
		UpdateVisualizations();
	}

	public void Set(SimpleColor color)
	{
		Color.Set(color);
		ColorUpdated?.Invoke();
		UpdateVisualizations();
	}

	public void SetColorFromHueAndColorBox()
	{
		Color.SetHSVA(hueSlider.HValue, colorBox.SValue, colorBox.VValue, aComponent.Value);
		ColorUpdated?.Invoke();
		UpdateVisualizations();
	}

	public void SetColorFromComponentSliders()
	{
		Color.SetRGBA(rComponent.Value, gComponent.Value, bComponent.Value, aComponent.Value);
		ColorUpdated?.Invoke();
		UpdateVisualizations();
	}

	public void SetColorFromHexInput()
	{
		if (!hexInput.Color.HasValue)
			return;

		Color.Set(hexInput.Color.Value);
		ColorUpdated?.Invoke();
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
