using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class DitherTool : DrawingTool
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
		{
			if (Mouse.IsPressed(combination))
			{
				QuickPencilType altValue = MouseColorInputMap[combination] switch
				{
					QuickPencilType.Primary => QuickPencilType.AltPrimary,
					QuickPencilType.Secondary => QuickPencilType.AltSecondary,
					QuickPencilType.AltPrimary => QuickPencilType.Primary,
					QuickPencilType.AltSecondary => QuickPencilType.Secondary,
					_ => QuickPencilType.Primary
				};

				Color color = Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor;
				Color altColor = Artist.GetQuickPencilColor(altValue).GodotColor;

				if (Type == ShapeType.Round)
					Brush.DitherLine(MousePixelPos, OldMousePixelPos, color, altColor, HistoryAction);
				else
					Brush.DitherLineOfSquares(MousePixelPos, OldMousePixelPos, color, altColor, HistoryAction);
			}
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			QuickPencilType altValue = value switch
			{
				QuickPencilType.Primary => QuickPencilType.AltPrimary,
				QuickPencilType.Secondary => QuickPencilType.AltSecondary,
				QuickPencilType.AltPrimary => QuickPencilType.Primary,
				QuickPencilType.AltSecondary => QuickPencilType.Secondary,
				_ => QuickPencilType.Primary
			};

			Color color = Artist.GetQuickPencilColor(value).GodotColor;
			Color altColor = Artist.GetQuickPencilColor(altValue).GodotColor;

			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawDither, Canvas.CurrentLayer.Id, Canvas.CurrentFrame.Id);
			Brush.Dither(MousePixelPos, color, altColor, Type == ShapeType.Square, HistoryAction);
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

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Selection.Clear();
	}
}
