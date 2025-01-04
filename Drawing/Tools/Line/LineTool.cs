using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class LineTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsDrawing { get; set; }

	public ShapeType Type { get; set; } = ShapeType.Round;

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; } = [];

	private void RedrawPencilPreview()
	{
		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);
			PencilPreviewPixels?.Clear();
			if (Global.Settings.PencilPreview)
			{
				Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
				PencilPreviewPixels.Add(MousePixelPos);
			}

			if (!IsDrawing)
				return;

			if (Type == ShapeType.Round)
				PencilPreviewPixels.AddRange(Brush.Line(Pos1, MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null));
			else
				PencilPreviewPixels.AddRange(Brush.LineOfSquares(Pos1, MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null));
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

	public override void Reset() => IsDrawing = false;

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (!IsDrawing && MouseColorInputMap.TryGetValue(combination, out _))
		{
			Pos1 = MousePixelPos;
			IsDrawing = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!IsDrawing)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			DrawHistoryAction historyAction = new(HistoryActionType.DrawLine, Canvas.CurrentLayer.Id, Canvas.CurrentFrame.Id);
			if (Type == ShapeType.Round)
				Brush.Line(Pos1, MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
					BrushPixelType.Normal, historyAction);
			else
				Brush.LineOfSquares(Pos1, MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
					BrushPixelType.Normal, historyAction);

			Canvas.History.AddAction(historyAction);
			IsDrawing = false;
			Canvas.ClearOverlay(OverlayType.EffectArea);
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			if (IsDrawing)
			{
				IsDrawing = false;
				Canvas.ClearOverlay(OverlayType.EffectArea);
			}
			else
				Selection.Clear();
		}
	}
}
