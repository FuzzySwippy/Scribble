using System;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class Selection
{
	private Canvas Canvas => Global.Canvas;
	private Vector2I Size { get; }
	private Vector2I DefaultOffset { get; }

	public bool HasSelection { get; private set; }

	private Vector2I offset;
	public Vector2I Offset
	{
		get => offset;
		set
		{
			offset = new(Mathf.Clamp(value.X, DefaultOffset.X * 2, 0),
				Mathf.Clamp(value.Y, DefaultOffset.Y * 2, 0));
			Update();
		}
	}

	private int SelectedPixelCount { get; set; }
	private bool[,] SelectedPixels { get; }
	private Color[,] SelectedColors { get; set; }
	private bool HasSelectedColors { get; set; }

	public bool MouseOnSelection
	{
		get
		{
			Vector2I mousePos = Global.Canvas.Drawing.MousePixelPos - Offset;
			return HasSelection && mousePos.X >= 0 && mousePos.Y >= 0 &&
				mousePos.X < Size.X && mousePos.Y < Size.Y && SelectedPixels[mousePos.X, mousePos.Y];
		}
	}

	private SelectionMovedHistoryAction SelectionMovedHistoryAction { get; set; }

	/// <summary>
	/// Checks if any part of the selection is in bounds.
	/// </summary>
	public bool InBounds
	{
		get
		{
			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
				{
					if (!SelectedPixels[x, y])
						continue;

					Vector2I pos = new Vector2I(x, y) + Offset;
					if (pos.X >= 0 && pos.Y >= 0 && pos.X < Canvas.Size.X && pos.Y < Canvas.Size.Y)
						return true;
				}
			}
			return false;
		}
	}

	public Selection(Vector2I areaSize)
	{
		Size = areaSize * 3;
		DefaultOffset = -areaSize;
		Offset = DefaultOffset;
		SelectedPixels = new bool[Size.X, Size.Y];
		SelectedColors = new Color[Size.X, Size.Y];
	}

	public void Clear()
	{
		SelectionClearHistoryAction historyAction = new(Offset);

		HasSelection = false;
		Offset = DefaultOffset;
		SelectedPixelCount = 0;

		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				if (SelectedPixels[x, y])
					historyAction.AddSelection(new Vector2I(x, y) + Offset);
				SelectedPixels[x, y] = false;
			}
		}
		Update();
		Canvas.History.AddAction(historyAction);
	}

	public bool SetPixel(Vector2I pos, bool selected = true)
	{
		pos -= Offset;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return false;

		SelectedPixels[pos.X, pos.Y] = selected;
		SelectedPixelCount += selected ? 1 : -1;
		HasSelection = SelectedPixelCount > 0;
		return true;
	}

	public bool TryGetPixel(Vector2I pos, out bool selected)
	{
		pos -= Offset;
		selected = false;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return false;

		selected = SelectedPixels[pos.X, pos.Y];
		return true;
	}

	public void SetArea(Vector2I pos1, Vector2I pos2, bool selected = true)
	{
		Vector2I min = new(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
		Vector2I max = new(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y));

		SelectionChangedHistoryAction historyAction = new();

		for (int x = min.X; x <= max.X; x++)
		{
			for (int y = min.Y; y <= max.Y; y++)
			{
				if (TryGetPixel(new Vector2I(x, y), out bool current))
				{
					if (current == selected)
						continue;

					historyAction.AddSelectionChange(new(new Vector2I(x, y), current, selected));
					SetPixel(new Vector2I(x, y), selected);
				}
			}
		}
		Update();

		Canvas.History.AddAction(historyAction);
	}

	public void Update()
	{
		if (SelectedPixels == null)
			return;

		Canvas.ClearOverlay(OverlayType.Selection);
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				if (Canvas.Drawing.DrawingTool?.SelectionTool == true)
				{
					if (SelectedPixels[x, y])
						Canvas.SetOverlayPixel(new Vector2I(x, y) + Offset, SelectedColors[x, y],
							OverlayType.Selection);
				}
				else if (Canvas.Selection.HasSelection && !SelectedPixels[x, y])
					Canvas.SetOverlayPixel(new Vector2I(x, y) + Offset, SelectedColors[x, y],
							OverlayType.NoSelection);
	}

	public bool IsSelectedPixel(Vector2I pos)
	{
		pos -= Offset;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return false;

		return HasSelection && SelectedPixels[pos.X, pos.Y];
	}

	public void TakeSelectedColors()
	{
		SelectionMovedHistoryAction = new(Canvas.CurrentLayer.ID, Offset);

		SelectedColors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				if (!SelectedPixels[x, y])
					continue;

				Vector2I pos = new Vector2I(x, y) + Offset;
				SelectionMovedHistoryAction.AddSelectionPixel(pos, Canvas.GetPixel(pos));

				SelectedColors[x, y] = Canvas.GetPixel(pos);
				Canvas.SetPixel(pos, new());
			}
		}
		HasSelectedColors = true;
	}

	public void CommitSelectedColors()
	{
		SelectionMovedHistoryAction.NewOffset = Offset;

		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				if (!SelectedPixels[x, y])
					continue;

				if (HasSelectedColors)
				{
					Vector2I pos = new Vector2I(x, y) + Offset;
					SelectionMovedHistoryAction.AddOverwrittenPixel(
						new(pos, Canvas.GetPixel(pos), SelectedColors[x, y]));

					Canvas.SetPixel(pos, SelectedColors[x, y]);
				}
				SelectedColors[x, y] = new();
			}
		}

		Canvas.History.AddAction(SelectionMovedHistoryAction);
		SelectionMovedHistoryAction = null;
	}
}
