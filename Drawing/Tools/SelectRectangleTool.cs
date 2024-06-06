using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectRectangleTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsSelecting { get; set; }
	public bool HasSelection { get; set; }
	public Rect2I SelectionRect { get; set; }

	private bool MovingSelection { get; set; }
	private Vector2I SelectionPivot { get; set; }

	private MouseButton CancelButton { get; } = MouseButton.Right;
	private MouseButton SelectButton { get; } = MouseButton.Left;

	public override void Reset()
	{
		IsSelecting = false;
		HasSelection = false;
		MovingSelection = false;
		Canvas.ClearOverlay();
		SetStatusText(true);
	}

	public override void MouseMoveUpdate()
	{
		if (IsSelecting)
		{
			Canvas.ClearOverlay();
			Brush.Rectangle(Pos1, MousePixelPos, new(), true);

			SetSelectionRectFromMousePositions();
		}
		else if (MovingSelection)
		{
			Canvas.ClearOverlay();
			Vector2I newPos = MousePixelPos - SelectionPivot;
			Brush.Rectangle(newPos, newPos + SelectionRect.Size, new(), true);
			SelectionRect = new Rect2I(newPos, SelectionRect.Size);
			SetStatusText();
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MovingSelection)
			return;

		if (combination.button == CancelButton)
		{
			Reset();
			return;
		}
		else if (combination.button == SelectButton)
		{
			if (HasSelection)
			{
				if (SelectionRectHasPoint(MousePixelPos))
				{
					MovingSelection = true;
					SelectionPivot = MousePixelPos - SelectionRect.Position;
					return;
				}

				Reset();
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
			if (!new Rect2I(Vector2I.Zero, Canvas.Size).Intersects(SelectionRect))
				Reset();
			return;
		}

		if (!Spacer.MouseInBounds || !IsSelecting)
			return;

		IsSelecting = false;
		HasSelection = true;

		SetSelectionRectFromMousePositions();

		if (!new Rect2I(Vector2I.Zero, Canvas.Size).Intersects(SelectionRect))
			Reset();
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Reset();
	}

	private bool SelectionRectHasPoint(Vector2I point) =>
		new Rect2I(SelectionRect.Position, SelectionRect.Size + Vector2I.One).HasPoint(point);

	private void SetStatusText(bool clear = false)
	{
		Status.Set("selection_pos", clear ? "" : SelectionRect.Position);
		Status.Set("Selection_size", clear ? "" : SelectionRect.Size);
	}

	private void SetSelectionRectFromMousePositions()
	{
		Vector2I rectPos = new(Mathf.Min(Pos1.X, MousePixelPos.X), Mathf.Min(Pos1.Y, MousePixelPos.Y));
		Vector2I rectSize = new(
			Mathf.Abs(Mathf.Min(Pos1.X, MousePixelPos.X) - Mathf.Max(Pos1.X, MousePixelPos.X)),
			Mathf.Abs(Mathf.Min(Pos1.Y, MousePixelPos.Y) - Mathf.Max(Pos1.Y, MousePixelPos.Y)));
		SelectionRect = new Rect2I(rectPos, rectSize);

		SetStatusText();
	}
}
