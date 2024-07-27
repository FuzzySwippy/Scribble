using System;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class ColorSelector : Control
{
	private ColorRect colorPreview;

	public Color Color
	{
		get => colorPreview.Modulate;
		set => colorPreview.Modulate = value;
	}

	public event Action<Color> ColorChanged;

	public override void _Ready() =>
		colorPreview = this.GetGrandChild<ColorRect>(3);

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
			SetupColorInput();
	}

	private void SetupColorInput()
	{
		Global.FloatingColorInput.Show(GetGlobalMousePosition(), Color);
		Global.FloatingColorInput.ColorChanged += OnColorChanged;
		Global.FloatingColorInput.Closed += CleanupColorInput;
	}

	private void CleanupColorInput()
	{
		Global.FloatingColorInput.ColorChanged -= OnColorChanged;
		Global.FloatingColorInput.Closed -= CleanupColorInput;
	}

	private void OnColorChanged(Color color)
	{
		Color = color;
		ColorChanged?.Invoke(color);
	}
}
