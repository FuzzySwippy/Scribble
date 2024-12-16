using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class DrawSelectionTool : DrawingTool
{
	private MouseButton DrawButton { get; } = MouseButton.Left;
	private MouseButton EraseButton { get; } = MouseButton.Right;

	private SelectionChangedHistoryAction HistoryAction { get; set; }

	public DrawSelectionTool() =>
		SelectionTool = true;

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

	private void RedrawPencilPreview()
	{
		if (!Global.Settings.PencilPreview)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);
			PencilPreviewPixels = Brush.Pencil(MousePixelPos, new(), false, BrushPixelType.EffectAreaOverlay, null);
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

		if (Mouse.IsPressed(DrawButton))
		{
			Brush.Line(MousePixelPos, OldMousePixelPos, new(), BrushPixelType.Selection, HistoryAction);
			Selection.Update();
		}
		else if (Mouse.IsPressed(EraseButton))
		{
			Brush.Line(MousePixelPos, OldMousePixelPos, new(), BrushPixelType.Deselection, HistoryAction);
			Selection.Update();
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == DrawButton)
		{
			HistoryAction = new();
			Brush.Pencil(MousePixelPos, new(), false, BrushPixelType.Selection, HistoryAction);
		}
		else if (combination.button == EraseButton)
		{
			HistoryAction = new();
			Brush.Pencil(MousePixelPos, new(), false, BrushPixelType.Deselection, HistoryAction);
		}
		Selection.Update();
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (HistoryAction == null)
			return;

		if (combination.button == DrawButton || combination.button == EraseButton)
		{
			Canvas.History.AddAction(HistoryAction);
			HistoryAction = null;
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Selection.Clear();
	}
}
