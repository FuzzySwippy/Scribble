using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class PencilTool : DrawingTool
{
	private DrawHistoryAction HistoryAction { get; set; }
	private bool Drawing { get; set; }

	//Properties
	public Pencil.Type Type { get; set; } = Pencil.Type.Round;

	public override void MouseMoveUpdate()
	{
		if (!Drawing)
			return;

		foreach (MouseCombination combination in MouseColorInputMap.Keys)
			if (Mouse.IsPressed(combination))
				if (Type == Pencil.Type.Round)
					Brush.Line(MousePixelPos, OldMousePixelPos,
						Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
						BrushPixelType.Normal, HistoryAction);
				else
					Brush.LineOfSquares(MousePixelPos, OldMousePixelPos,
						Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
						BrushPixelType.Normal, HistoryAction);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawPencil, Canvas.CurrentLayer.ID);
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
				Type == Pencil.Type.Square, BrushPixelType.Normal, HistoryAction);
			Drawing = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		Drawing = false;

		if (HistoryAction == null)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out _))
		{
			Canvas.History.AddAction(HistoryAction);
			HistoryAction = null;
		}
	}
}
