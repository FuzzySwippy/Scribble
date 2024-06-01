using System.Collections.Generic;
using Godot;
using Scribble.Drawing.Tools;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing;

public class DrawingController
{
	private Canvas Canvas { get; }

	private Artist Artist { get; }
	public Brush Brush => Artist.Brush;

	private DrawingToolType toolType;
	public DrawingToolType ToolType
	{
		get => toolType;
		set
		{
			toolType = value;
			DebugInfo.Set("draw_tool", toolType);
		}
	}

	private DrawingTool DrawingTool { get; set; }

	//Input
	public static Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap { get; } = new()
	{
		{ new (MouseButton.Left), QuickPencilType.Primary },
		{ new (MouseButton.Right), QuickPencilType.Secondary },
		{ new (MouseButton.Left, KeyModifierMask.MaskCtrl), QuickPencilType.AltPrimary },
		{ new (MouseButton.Right, KeyModifierMask.MaskCtrl), QuickPencilType.AltSecondary },
	};

	//Pixel
	private Vector2I OldMousePixelPos { get; set; } = Vector2I.One * -1;
	public Vector2I MousePixelPos { get; set; }

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
	}

	public void Update()
	{
		MousePixelPos = (Mouse.GlobalPosition / Canvas.PixelSize).ToVector2I();

		if (OldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
			{
				foreach (MouseCombination combination in MouseColorInputMap.Keys)
					if (Mouse.IsPressed(combination))
						Brush.Line(MousePixelPos, OldMousePixelPos,
						Brush.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor);
			}
			OldMousePixelPos = MousePixelPos;
		}

		Status.Set("pixel_pos", MousePixelPos);
	}
}
