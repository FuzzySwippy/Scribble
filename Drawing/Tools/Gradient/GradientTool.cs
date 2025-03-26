using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Drawing.Tools.Gradient;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class GradientTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsDrawing { get; set; }
	private QuickPencilType MoveValue { get; set; }

	public GradientType Type { get; set; } = GradientType.Linear;

	private void RedrawPencilPreview()
	{
		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlay(OverlayType.Preview);
			if (Global.Settings.PencilPreview)
				Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);

			if (!IsDrawing)
				return;

			if (Pos1 == MousePixelPos)
				return;

			Brush.Gradient(Pos1, MousePixelPos, Artist.GetQuickPencilColor(MoveValue).GodotColor,
				Artist.GetQuickPencilAltColor(MoveValue).GodotColor, Type, BrushPixelType.Preview, null);
		}
	}

	public override void Selected() =>
		RedrawPencilPreview();

	public override void Deselected() =>
		Canvas.ClearOverlay(OverlayType.Preview);

	public override void MouseMoveUpdate() =>
		RedrawPencilPreview();

	public override void Reset() => IsDrawing = false;

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (!IsDrawing && MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			MoveValue = value;
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
			if (Pos1 != MousePixelPos)
			{
				DrawHistoryAction historyAction = new(HistoryActionType.DrawGradient, Canvas.CurrentLayer.Id, Canvas.CurrentFrame.Id);
				Brush.Gradient(Pos1, MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, Artist.GetQuickPencilAltColor(value).GodotColor, Type, BrushPixelType.Normal, historyAction);

				Canvas.History.AddAction(historyAction);
			}
			IsDrawing = false;
			Canvas.ClearOverlay(OverlayType.Preview);
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			if (IsDrawing)
			{
				IsDrawing = false;
				Canvas.ClearOverlay(OverlayType.Preview);
			}
			else
				Selection.Clear();
		}
	}
}
