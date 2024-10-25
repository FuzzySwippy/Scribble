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

	public override void Update()
	{
		Canvas.ClearOverlay(OverlayType.EffectArea);
		if (Global.Settings.PencilPreview)
			Brush.Pencil(MousePixelPos, new(), Type != ShapeType.Round, BrushPixelType.EffectAreaOverlay, null);
	}

	public override void MouseMoveUpdate()
	{
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

			HistoryAction = new DrawHistoryAction(HistoryActionType.DrawDither, Canvas.CurrentLayer.ID);
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
}