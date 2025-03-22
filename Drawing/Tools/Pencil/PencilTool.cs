using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class PencilTool : DrawingTool
{
	private DrawHistoryAction HistoryAction { get; set; }
	private bool Drawing { get; set; }

	private MouseCombination SampleColorCombination { get; } = new(MouseButton.Middle, KeyModifierMask.MaskCtrl);
	// Used to ignore canvas drag
	private bool SampleColor { get; set; }

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
			PencilPreviewPixels = Brush.Pencil(MousePixelPos, new(), Type, BrushPixelType.EffectAreaOverlay, null);
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
				Brush.Line(MousePixelPos, OldMousePixelPos, Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor, Type, BrushPixelType.Normal, HistoryAction);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination == SampleColorCombination)
			SampleColor = true;
		else if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawPencil, Canvas.CurrentLayer.Id, Canvas.CurrentFrame.Id);
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, Type, BrushPixelType.Normal, HistoryAction);
			Drawing = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		Drawing = false;

		if (combination == SampleColorCombination && SampleColor)
		{
			Global.QuickPencils.SelectedType = QuickPencilType.Primary;
			Brush.SampleColor(MousePixelPos, false, true);
			SampleColor = false;
			return;
		}

		if (HistoryAction == null)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out _))
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

	public override void MouseDrag(MouseCombination combination, Vector2 position, Vector2 positionChange, Vector2 velocity) =>
		SampleColor = false;
}
