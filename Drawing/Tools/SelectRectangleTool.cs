using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectRectangleTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsSelecting { get; set; }
	private bool IsDeselecting { get; set; }

	private bool MovingSelection { get; set; }
	private Vector2I SelectionMoveStart { get; set; }

	private MouseButton SelectButton { get; } = MouseButton.Left;
	private MouseButton DeselectButton { get; } = MouseButton.Right;
	private Key ClearPixelsKey { get; } = Key.Delete;

	public SelectRectangleTool()
	{
		ResetOnSelection = false;
		SelectionTool = true;
	}

	public override void Reset()
	{
		IsSelecting = false;
		IsDeselecting = false;
		MovingSelection = false;
		Canvas.ClearOverlay(OverlayType.EffectArea);
		SetStatusText(true);
		Canvas.Selection.Clear();
	}

	public override void MouseMoveUpdate()
	{
		Canvas.ClearOverlay(OverlayType.EffectArea);
		if (IsSelecting || IsDeselecting)
		{
			Brush.Rectangle(Pos1, MousePixelPos, new(),
				IsSelecting ? BrushPixelType.EffectAreaOverlay : BrushPixelType.EffectAreaOverlayAlt,
				false, null);
			SetStatusText();
		}
		else if (MovingSelection)
		{
			Canvas.History.AddAction(new SelectionOffsetChangedHistoryAction(
				Selection.Offset, MousePixelPos - SelectionMoveStart));
			Selection.Offset = MousePixelPos - SelectionMoveStart;
		}
		else if (Global.Settings.PencilPreview)
			Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MovingSelection)
			return;

		if (combination.button == SelectButton)
		{
			if (Selection.HasSelection && Selection.MouseOnSelection)
			{
				MovingSelection = true;
				SelectionMoveStart = MousePixelPos - Selection.Offset;
				return;
			}

			Pos1 = MousePixelPos;
			IsSelecting = true;
		}
		else if (combination.button == DeselectButton)
		{
			Pos1 = MousePixelPos;
			IsDeselecting = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (MovingSelection)
		{
			MovingSelection = false;
			if (!Selection.InBounds)
				Reset();
			return;
		}

		if (!Spacer.MouseInBounds || !IsSelecting && !IsDeselecting)
			return;

		SetSelectionRectFromMousePositions();
		IsSelecting = false;
		IsDeselecting = false;
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			if (IsSelecting || IsDeselecting)
			{
				Canvas.ClearOverlay(OverlayType.EffectArea);
				SetStatusText(true);
				IsSelecting = false;
				IsDeselecting = false;
			}
			else
				Reset();
		}
		else if (combination.key == ClearPixelsKey)
			Selection.ClearPixels();
	}

	private void SetStatusText(bool clear = false)
	{
		int x1 = Mathf.Min(Pos1.X, MousePixelPos.X);
		int y1 = Mathf.Min(Pos1.Y, MousePixelPos.Y);
		int x2 = Mathf.Max(Pos1.X, MousePixelPos.X) + 1;
		int y2 = Mathf.Max(Pos1.Y, MousePixelPos.Y) + 1;
		Rect2I selectionRect = new(new Vector2I(x1, y1), new Vector2I(x2, y2) - new Vector2I(x1, y1));

		Status.Set("selection_pos", clear ? "" : Pos1);
		Status.Set("Selection_size", clear ? "" : selectionRect.Size);
	}

	private void SetSelectionRectFromMousePositions()
	{
		Selection.SetArea(Pos1, MousePixelPos, IsSelecting);
		Canvas.ClearOverlay(OverlayType.EffectArea);

		SetStatusText(true);
	}
}
