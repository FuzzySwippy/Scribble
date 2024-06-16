using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class PencilRoundTool : DrawingTool
{
	private DrawHistoryAction HistoryAction { get; set; }

	public override void MouseMoveUpdate()
	{
		foreach (MouseCombination combination in MouseColorInputMap.Keys)
			if (Mouse.IsPressed(combination))
				Brush.Line(MousePixelPos, OldMousePixelPos,
					Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
					BrushPixelType.Normal, HistoryAction);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawPencilRound, Canvas.CurrentLayer.ID);
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
				false, BrushPixelType.Normal, HistoryAction);
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (HistoryAction == null)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out _))
		{
			Canvas.History.AddAction(HistoryAction);
			HistoryAction = null;
		}
	}
}
