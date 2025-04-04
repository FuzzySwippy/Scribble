using System;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class FloatingColorInput : CanvasLayer
{
	private ColorInput ColorInput { get; set; }
	private Control ColorInputParent { get; set; }

	public event Action Closed;
	public event Action<Color> ColorChanged;

	public override void _Ready()
	{
		ColorInputParent = GetChild<Control>(0);
		ColorInput = ColorInputParent.GetChild<ColorInput>(0);

		ColorInput.ColorUpdated +=
			() => ColorChanged?.Invoke(ColorInput.Color.GodotColor);

		Main.Ready += Hide;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed &&
			!ColorInputParent.GetGlobalRect().HasPoint(mouseButton.Position))
			Close();
	}

	private void Close()
	{
		Hide();
		Closed?.Invoke();
	}

	public void Show(Vector2 position, Color color)
	{
		if (Visible)
			Close();

		ColorInputParent.Position = position;

		//Keep the color input inside the viewport
		if (ColorInputParent.Position.Y + ColorInputParent.Size.Y > Main.ViewportRect.Size.Y)
			ColorInputParent.Position -= new Vector2(0, ColorInputParent.Size.Y);
		if (ColorInputParent.Position.X + ColorInputParent.Size.X > Main.ViewportRect.Size.X)
			ColorInputParent.Position -= new Vector2(ColorInputParent.Size.X, 0);

		ColorInput.SetColor(color);
		Show();
	}

	public void SetColor(Color color) =>
		ColorInput.SetColor(color);
}
