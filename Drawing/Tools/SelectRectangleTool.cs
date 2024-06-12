using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectRectangleTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsSelecting { get; set; }

	private bool MovingSelection { get; set; }
	private Vector2I SelectionMoveStart { get; set; }

	private MouseButton CancelButton { get; } = MouseButton.Right;
	private MouseButton SelectButton { get; } = MouseButton.Left;

	public SelectRectangleTool()
	{
		ResetOnSelection = false;
		SelectionTool = true;
	}

	public override void Reset()
	{
		IsSelecting = false;
		MovingSelection = false;
		Canvas.ClearOverlay(OverlayType.EffectArea);
		SetStatusText(true);
		Canvas.Selection.Clear();
	}

	public override void MouseMoveUpdate()
	{
		if (IsSelecting)
		{
			Canvas.ClearOverlay(OverlayType.EffectArea);
			Brush.Rectangle(Pos1, MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, false, null);
			SetStatusText();
		}
		else if (MovingSelection)
			Selection.Offset = MousePixelPos - SelectionMoveStart;
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == CancelButton)
		{
			Reset();
			return;
		}

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

		if (!Spacer.MouseInBounds || !IsSelecting)
			return;

		IsSelecting = false;

		SetSelectionRectFromMousePositions();
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Reset();
	}

	private void SetStatusText(bool clear = false)
	{
		int x1 = Mathf.Min(Pos1.X, MousePixelPos.X);
		int y1 = Mathf.Min(Pos1.Y, MousePixelPos.Y);
		int x2 = Mathf.Max(Pos1.X, MousePixelPos.X);
		int y2 = Mathf.Max(Pos1.Y, MousePixelPos.Y);
		Rect2I selectionRect = new(new Vector2I(x1, y1), new Vector2I(x2, y2) - new Vector2I(x1, y1));

		Status.Set("selection_pos", clear ? "" : Pos1);
		Status.Set("Selection_size", clear ? "" : selectionRect.Size);
	}

	private void SetSelectionRectFromMousePositions()
	{
		Selection.SetArea(Pos1, MousePixelPos);
		Canvas.ClearOverlay(OverlayType.EffectArea);

		SetStatusText(true);
	}
}
