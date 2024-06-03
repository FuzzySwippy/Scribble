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

	public Artist Artist { get; }

	private DrawingToolType toolType;
	public DrawingToolType ToolType
	{
		get => toolType;
		set
		{
			toolType = value;
			DrawingTool = DrawingTools.TryGetValue(toolType, out DrawingTool tool) ? tool : null;
			DebugInfo.Set("draw_tool", DrawingTool == null ? "null" : toolType);
		}
	}

	private DrawingTool DrawingTool { get; set; }

	private Dictionary<DrawingToolType, DrawingTool> DrawingTools { get; }

	//Input
	public Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap { get; } = new()
	{
		{ new (MouseButton.Left), QuickPencilType.Primary },
		{ new (MouseButton.Right), QuickPencilType.Secondary },
		{ new (MouseButton.Left, KeyModifierMask.MaskCtrl), QuickPencilType.AltPrimary },
		{ new (MouseButton.Right, KeyModifierMask.MaskCtrl), QuickPencilType.AltSecondary },
	};

	//Pixel
	public Vector2I OldMousePixelPos { get; set; } = Vector2I.One * -1;
	public Vector2I MousePixelPos { get; set; }

	public DrawingController(Canvas canvas, Artist artist)
	{
		Canvas = canvas;
		Artist = artist;

		Mouse.ButtonDown += MouseDown;

		//Init drawing tools
		DrawingTools = new()
		{
			{ DrawingToolType.PencilRound, new PencilRoundTool() },
			{ DrawingToolType.PencilSquare, new PencilSquareTool() },
			{ DrawingToolType.Sample, new SampleTool() }
		};

		//Update tool type
		ToolType = toolType;
		DebugInfo.Set("draw_tool", DrawingTool == null ? "null" : toolType);
	}

	private void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		DrawingTool?.MouseDown(combination, position);
	}

	public void Update()
	{
		MousePixelPos = (Mouse.GlobalPosition / Canvas.PixelSize).ToVector2I();

		if (OldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
				DrawingTool?.Update();
			OldMousePixelPos = MousePixelPos;
		}

		Status.Set("pixel_pos", MousePixelPos);
	}
}
