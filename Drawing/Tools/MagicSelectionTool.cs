using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class MagicSelectionTool : DrawingTool
{
	private MouseButton DrawButton { get; } = MouseButton.Left;
	private MouseButton EraseButton { get; } = MouseButton.Right;

	private SelectionChangedHistoryAction HistoryAction { get; set; }

	public MagicSelectionTool() =>
		SelectionTool = true;

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == DrawButton)
		{
			HistoryAction = new();
			Brush.Flood(MousePixelPos, new(), HistoryAction, BrushPixelType.Selection);
			Selection.Update();
			Canvas.History.AddAction(HistoryAction);
		}
		else if (combination.button == EraseButton)
		{
			HistoryAction = new();
			Brush.Flood(MousePixelPos, new(), HistoryAction, BrushPixelType.Deselection);
			Selection.Update();
			Canvas.History.AddAction(HistoryAction);
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Selection.Clear();
	}
}
