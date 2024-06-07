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

	public bool MouseOnSelection
	{
		get
		{
			Vector2I mousePos = Global.Canvas.Drawing.MousePixelPos - Offset;
			return HasSelection && mousePos.X >= 0 && mousePos.Y >= 0 &&
				mousePos.X < Size.X && mousePos.Y < Size.Y && SelectedPixels[mousePos.X, mousePos.Y];
		}
	}

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
	}

	public void Clear()
	{
		HasSelection = false;
		Offset = DefaultOffset;
		SelectedPixelCount = 0;
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				SelectedPixels[x, y] = false;
			}
		}
		Update();
	}

	public void SetPixel(Vector2I pos, bool selected = true)
	{
		pos -= Offset;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return;

		SelectedPixels[pos.X, pos.Y] = selected;
		SelectedPixelCount += selected ? 1 : -1;
		HasSelection = SelectedPixelCount > 0;
	}

	public void SetArea(Vector2I pos1, Vector2I pos2, bool selected = true)
	{
		Vector2I min = new(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
		Vector2I max = new(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y));

		for (int x = min.X; x <= max.X; x++)
		{
			for (int y = min.Y; y <= max.Y; y++)
			{
				SetPixel(new Vector2I(x, y), selected);
			}
		}
		Update();
	}

	public void Update()
	{
		if (SelectedPixels == null)
			return;

		Canvas.ClearOverlay(OverlayType.Selection);
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				if (SelectedPixels[x, y])
					Canvas.SetOverlayPixel(new Vector2I(x, y) + Offset, new(), OverlayType.Selection);
	}

	public bool IsSelectedPixel(Vector2I pos)
	{
		pos -= Offset;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return false;

		return HasSelection && SelectedPixels[pos.X, pos.Y];
	}
}
