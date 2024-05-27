using System.Collections.Generic;
using Godot;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing;

public class DrawingController
{
	private Canvas Canvas { get; }

	private Artist Artist { get; }
	private Brush Brush => Artist.Brush;

	//Input
	private Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap { get; } = new()
	{
		{ new (MouseButton.Left), QuickPencilType.Primary },
		{ new (MouseButton.Right), QuickPencilType.Secondary },
		{ new (MouseButton.Left, KeyModifierMask.MaskCtrl), QuickPencilType.AltPrimary },
		{ new (MouseButton.Right, KeyModifierMask.MaskCtrl), QuickPencilType.AltSecondary },
	};

	private MouseButton SampleColorButton { get; } = MouseButton.Middle;

	//Pixel
	private Vector2I oldMousePixelPos = Vector2I.One * -1;
	private Vector2I frameMousePixelPos;
	public Vector2I MousePixelPos => frameMousePixelPos;

	public DrawingController(Canvas canvas, Artist artist)
	{
		Canvas = canvas;
		Artist = artist;

		Mouse.ButtonDown += MouseDown;
	}

	private void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
			Brush.Pencil(MousePixelPos, Brush.GetQuickPencilColor(value).GodotColor);
		else if (combination.button == SampleColorButton)
			Brush.SampleColor(MousePixelPos);
	}

	public void Update()
	{
		frameMousePixelPos = (Mouse.GlobalPosition / Canvas.PixelSize).ToVector2I();

		if (oldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
			{
				//if (Mouse.IsPressed())

				foreach (MouseCombination combination in MouseColorInputMap.Keys)
					if (Mouse.IsPressed(combination))
						Brush.Line(MousePixelPos, oldMousePixelPos,
						Brush.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor);
			}
			oldMousePixelPos = MousePixelPos;
		}

		Status.Set("pixel_pos", MousePixelPos);
	}
}
