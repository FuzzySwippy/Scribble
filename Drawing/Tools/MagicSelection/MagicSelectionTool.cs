using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class MagicSelectionTool : DrawingTool
{
	private MouseButton DrawButton { get; } = MouseButton.Left;
	private MouseButton EraseButton { get; } = MouseButton.Right;

	private SelectionChangedHistoryAction HistoryAction { get; set; }

	//Properties
	public float Threshold { get; set; } = 0;
	public bool Diagonal { get; set; } = false;
	public bool MergeLayers { get; set; } = false;

	public MagicSelectionTool() =>
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
			Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
			PencilPreviewPixels = new List<Vector2I> { MousePixelPos };
		}
	}


	public override void Selected() =>
		RedrawPencilPreview();

	public override void Deselected() =>
		Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);

	public override void SizeChanged(int size) =>
		RedrawPencilPreview();

	public override void MouseMoveUpdate() =>
		RedrawPencilPreview();

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == DrawButton)
		{
			HistoryAction = new();
			Brush.Flood(MousePixelPos, new(), Threshold, Diagonal, MergeLayers, HistoryAction, BrushPixelType.Selection);
			Selection.Update();
			Canvas.History.AddAction(HistoryAction);
		}
		else if (combination.button == EraseButton)
		{
			HistoryAction = new();
			Brush.Flood(MousePixelPos, new(), Threshold, Diagonal, MergeLayers, HistoryAction, BrushPixelType.Deselection);
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
