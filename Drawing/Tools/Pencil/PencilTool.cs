using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class PencilTool : DrawingTool
{
	private DrawHistoryAction HistoryAction { get; set; }
	private bool Drawing { get; set; }

	//Properties
	public ShapeType Type { get; set; } = ShapeType.Round;

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

	private void RedrawPencilPreview()
	{
		if (!Global.Settings.PencilPreview)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);
			PencilPreviewPixels = Brush.Pencil(MousePixelPos, new(), Type != ShapeType.Round, BrushPixelType.EffectAreaOverlay, null);
		}
	}


	public override void Selected() =>
		RedrawPencilPreview();

	public override void Deselected() =>
		Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);

	public override void SizeChanged(int size) =>
		RedrawPencilPreview();

	public override void MouseMoveUpdate()
	{
		RedrawPencilPreview();

		if (!Drawing)
			return;

		foreach (MouseCombination combination in MouseColorInputMap.Keys)
			if (Mouse.IsPressed(combination))
				if (Type == ShapeType.Round)
				{
					Brush.Line(MousePixelPos, OldMousePixelPos,
						Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
						BrushPixelType.Normal, HistoryAction);
					//Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
					//	false, BrushPixelType.Normal, HistoryAction);
				}
				else
				{
					Brush.LineOfSquares(MousePixelPos, OldMousePixelPos,
						Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
						BrushPixelType.Normal, HistoryAction);
					//Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
					//	true, BrushPixelType.Normal, HistoryAction);
				}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawPencil, Canvas.CurrentLayer.ID);
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
				Type == ShapeType.Square, BrushPixelType.Normal, HistoryAction);
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
