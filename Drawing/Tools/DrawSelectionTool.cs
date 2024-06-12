using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class DrawSelectionTool : DrawingTool
{
	private MouseButton DrawButton { get; } = MouseButton.Left;
	private MouseButton EraseButton { get; } = MouseButton.Right;

	public DrawSelectionTool() =>
		SelectionTool = true;

	public override void MouseMoveUpdate()
	{
		if (Mouse.IsPressed(DrawButton))
		{
			Brush.Line(MousePixelPos, OldMousePixelPos, new(), BrushPixelType.Selection, null);
			Selection.Update();
		}
		else if (Mouse.IsPressed(EraseButton))
		{
			Brush.Line(MousePixelPos, OldMousePixelPos, new(), BrushPixelType.Deselection, null);
			Selection.Update();
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == DrawButton)
			Brush.Pencil(MousePixelPos, new(), false, BrushPixelType.Selection, null);
		else if (combination.button == EraseButton)
			Brush.Pencil(MousePixelPos, new(), false, BrushPixelType.Deselection, null);
		Selection.Update();
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Selection.Clear();
	}
}
