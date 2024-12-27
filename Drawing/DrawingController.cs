using System;
using System.Collections.Generic;
using Godot;
using Scribble.Application;
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
			if (toolType == value)
				return;

			DrawingTool?.Deselected();

			DrawingTool = DrawingTools.TryGetValue(value, out DrawingTool tool) ? tool : null;
			toolType = value;
			if (DrawingTool?.ResetOnSelection == true)
				DrawingTool?.Reset();
			Canvas.Selection?.Update();
			Canvas.ClearOverlay(OverlayType.EffectArea);
			ToolTypeChanged?.Invoke(toolType);
			DebugInfo.Set("draw_tool", DrawingTool == null ? "null" : toolType);

			DrawingTool?.Selected();
		}
	}

	public event Action<DrawingToolType> ToolTypeChanged;

	public DrawingTool DrawingTool { get; private set; }

	private Dictionary<DrawingToolType, DrawingTool> DrawingTools { get; }

	//Input
	public Dictionary<MouseCombination, QuickPencilType> MouseColorInputMap { get; } = new()
	{
		{ new (MouseButton.Left), QuickPencilType.Primary },
		{ new (MouseButton.Right), QuickPencilType.Secondary },
		{ new (MouseButton.Left, KeyModifierMask.MaskCtrl), QuickPencilType.AltPrimary },
		{ new (MouseButton.Right, KeyModifierMask.MaskCtrl), QuickPencilType.AltSecondary },
	};

	public Key[] CancelKeys { get; } = [Key.Escape, Key.End];

	//Pixel
	public Vector2I OldMousePixelPos { get; set; } = Vector2I.One * -1;
	public Vector2I MousePixelPos { get; set; }


	private bool FocusEntered { get; set; }

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
		Keyboard.KeyUp += KeyUp;

		Main.WindowFocusEntered += () => FocusEntered = true;

		//Init drawing tools
		DrawingTools = new()
		{
			{ DrawingToolType.None, null },
			{ DrawingToolType.Pencil, new PencilTool() },
			{ DrawingToolType.Dither, new DitherTool() },
			{ DrawingToolType.Sample, new SampleTool() },
			{ DrawingToolType.Line, new LineTool() },
			{ DrawingToolType.Rectangle, new RectangleTool() },
			{ DrawingToolType.Flood, new FloodTool() },
			{ DrawingToolType.SelectRectangle, new SelectRectangleTool() },
			{ DrawingToolType.SelectionMove, new SelectionMoveTool() },
			{ DrawingToolType.DrawSelection, new DrawSelectionTool() },
			{ DrawingToolType.MagicSelection, new MagicSelectionTool() },
			{ DrawingToolType.SelectionRotate, new SelectionRotateTool() }
		};

		//Update tool type
		ToolType = Global.DefaultToolType;


		Brush.SizeChanged += SizeChanged;

		GD.Print("DrawingController initialized");
	}

	private void SizeChanged(int size) =>
		DrawingTool?.SizeChanged(size);

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

	private void KeyUp(KeyCombination combination) =>
	DrawingTool?.KeyUp(combination);

	public void Update()
	{
		MousePixelPos = (Mouse.GlobalPosition / Canvas.PixelSize).ToVector2I();

		//Fix for a line being drawn from the border when
		//starting to draw after the window was unfocused
		if (FocusEntered && Canvas.PixelInBounds(MousePixelPos))
		{
			OldMousePixelPos = MousePixelPos;
			FocusEntered = false;
		}

		if (OldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
				DrawingTool?.MouseMoveUpdate();
			OldMousePixelPos = MousePixelPos;
		}

		DrawingTool?.Update();

		Status.Set("pixel_pos", MousePixelPos);
	}

	public DrawingTool GetTool(DrawingToolType type) =>
		DrawingTools.TryGetValue(type, out DrawingTool tool) ? tool : null;
}
