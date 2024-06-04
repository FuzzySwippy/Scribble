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
			DrawingTool?.Reset();
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

	public Key[] CancelKeys { get; } = { Key.Escape, Key.Backspace };

	//Pixel
	public Vector2I OldMousePixelPos { get; set; } = Vector2I.One * -1;
	public Vector2I MousePixelPos { get; set; }

	public DrawingController(Canvas canvas, Artist artist)
	{
		Canvas = canvas;
		Artist = artist;

		Mouse.ButtonDown += MouseDown;
		Mouse.ButtonUp += MouseUp;
		Mouse.Drag += MouseDrag;
		Mouse.DragStart += MouseDragStart;
		Mouse.DragEnd += MouseDragEnd;
		Keyboard.KeyDown += KeyDown;

		//Init drawing tools
		DrawingTools = new()
		{
			{ DrawingToolType.PencilRound, new PencilRoundTool() },
			{ DrawingToolType.PencilSquare, new PencilSquareTool() },
			{ DrawingToolType.Sample, new SampleTool() },
			{ DrawingToolType.Line, new LineTool() },
			{ DrawingToolType.Flood, new FloodTool() },
			{ DrawingToolType.SelectRectangle, new SelectRectangleTool() }
		};

		//Update tool type
		ToolType = toolType;
		DebugInfo.Set("draw_tool", DrawingTool == null ? "null" : toolType);
	}

	private void MouseDown(MouseCombination combination, Vector2 position) =>
		DrawingTool?.MouseDown(combination, position);

	private void MouseUp(MouseCombination combination, Vector2 position) =>
		DrawingTool?.MouseUp(combination, position);

	private void MouseDrag(MouseCombination combination, Vector2 position,
		Vector2 positionChange, Vector2 velocity) =>
		DrawingTool?.MouseDrag(combination, position, positionChange, velocity);

	private void MouseDragStart(MouseCombination combination, Vector2 position,
		Vector2 positionChange, Vector2 velocity) =>
		DrawingTool?.MouseDragStart(combination, position, positionChange, velocity);

	private void MouseDragEnd(MouseCombination combination, Vector2 position,
		Vector2 positionChange, Vector2 velocity) =>
		DrawingTool?.MouseDragEnd(combination, position, positionChange, velocity);

	private void KeyDown(KeyCombination combination) =>
		DrawingTool?.KeyDown(combination);

	public void Update()
	{
		MousePixelPos = (Mouse.GlobalPosition / Canvas.PixelSize).ToVector2I();

		if (OldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
				DrawingTool?.MouseMoveUpdate();
			OldMousePixelPos = MousePixelPos;
		}

		DrawingTool?.Update();

		Status.Set("pixel_pos", MousePixelPos);
	}
}
