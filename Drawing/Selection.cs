using System;
using Godot;
using Scribble.Application;
using Scribble.UI;
using Math = System.Math;

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

	public Rect2I SelectionRect
	{
		get
		{
			if (!HasSelection)
				return new();

			Vector2I min = new(int.MaxValue, int.MaxValue);
			Vector2I max = new(int.MinValue, int.MinValue);

			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
				{
					if (!SelectedPixels[x, y])
						continue;

					Vector2I pos = new(x, y);
					min = min.Min(pos);
					max = max.Max(pos);
				}
			}

			return new Rect2I(min, max.X - min.X + 1, max.Y - min.Y + 1);
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

	public void Clear(bool recordHistory = true)
	{
		if (!HasSelection)
			return;

		SelectionClearHistoryAction historyAction = new(Offset);

		HasSelection = false;
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

		Offset = DefaultOffset;
		Update(true);

		if (recordHistory)
			Canvas.History.AddAction(historyAction);
	}

	/// <summary>
	/// Sets a pixel in the selection. Does not record history.
	/// </summary>
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

	public void SetPixelWithHistory(Vector2I pos, bool selected = true) =>
		SetArea(pos, pos, selected);

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

	public void Update(bool cleared = false)
	{
		if (SelectedPixels == null)
			return;

		Canvas.ClearOverlay(OverlayType.Selection);

		if (cleared)
			return;

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

	/// <summary>
	/// Copies/cuts the selection or the entire layer if no selection is available to the clipboard.
	/// </summary>
	/// <param name="cut">Cut</param>
	/// <returns><see langword="true"/> if the entire layer was copied/cut</returns>
	public bool Copy(bool cut = false)
	{
		bool layer = false;
		Rect2I selectionRect = SelectionRect;
		if (selectionRect.Size.X == 0 || selectionRect.Size.Y == 0)
		{
			selectionRect = new Rect2I(-Offset, Canvas.Size);
			layer = true;
		}

		CutHistoryAction historyAction = new(Canvas.CurrentLayer.ID);

		Image image = Image.CreateEmpty(selectionRect.Size.X, selectionRect.Size.Y, false, Image.Format.Rgba8);
		for (int x = 0; x < selectionRect.Size.X; x++)
		{
			for (int y = 0; y < selectionRect.Size.Y; y++)
			{
				Vector2I selectionPos = selectionRect.Position + new Vector2I(x, y);
				if (!SelectedPixels[selectionPos.X, selectionPos.Y])
					continue;

				Vector2I pos = selectionRect.Position + new Vector2I(x, y) + Offset;
				Color color = Canvas.GetPixel(pos);

				image.SetPixel(x, y, color);
				if (cut)
				{
					Canvas.SetPixel(pos, new());
					historyAction.AddPixelChange(pos, color);
				}
			}
		}

		string imageData = Convert.ToBase64String(image.SavePngToBuffer());
		DisplayServer.ClipboardSet(imageData);

		if (cut)
			Canvas.History.AddAction(historyAction);

		return layer;
	}

	public bool Paste()
	{
		Vector2I pastePos = Canvas.Drawing.MousePixelPos;
		if (!Spacer.MouseInBounds)
			pastePos = Vector2I.Zero;

		Image image = new();
		try
		{
			byte[] imageData = Convert.FromBase64String(DisplayServer.ClipboardGet());
			if (image.LoadPngFromBuffer(imageData) != Error.Ok)
				throw new Exception();
		}
		catch { return false; }

		Clear();

		Paste(pastePos, image);

		Canvas.History.AddAction(new PasteHistoryAction(Canvas.CurrentLayerIndex, pastePos, image));
		return true;
	}

	public void Paste(Vector2I mousePos, Image image)
	{
		Canvas.NewLayer(recordHistory: false);

		for (int x = 0; x < image.GetWidth(); x++)
		{
			for (int y = 0; y < image.GetHeight(); y++)
			{
				Vector2I pos = new Vector2I(x, y) + mousePos;
				if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
					continue;

				SetPixel(pos, true);
				Canvas.SetPixel(pos, image.GetPixel(x, y));
			}
		}

		Global.DrawingToolPanel.Select(DrawingToolType.SelectionMove);
	}
}
