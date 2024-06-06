using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class Selection
{
	private Canvas Canvas => Global.Canvas;
	private Vector2I Size { get; }

	public bool HasSelection { get; private set; }

	private Vector2I Offset { get; set; }
	private bool[,] SelectedPixels { get; }
	private bool[,] PreviewSelectedPixels { get; }

	public Selection(Vector2I areaSize)
	{
		Size = areaSize;
		SelectedPixels = new bool[Size.X, Size.Y];
	}

	public void Clear()
	{
		HasSelection = false;
		Offset = new();
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				SelectedPixels[x, y] = false;
				PreviewSelectedPixels[x, y] = false;
			}
		}
	}

	public void ClearPreview()
	{
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				PreviewSelectedPixels[x, y] = false;
	}

	public void SetPreviewPixel(Vector2I pos, bool selected) =>
		PreviewSelectedPixels[pos.X, pos.Y] = selected;

	public void CommitPreview()
	{
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				if (PreviewSelectedPixels[x, y])
					SelectedPixels[x, y] = true;
			}
		}
	}

	public void Update()
	{
		if (!HasSelection)
			return;

		Canvas.ClearOverlay();
		for (int x = 0; x < SelectedPixels.GetLength(0); x++)
		{
			for (int y = 0; y < SelectedPixels.GetLength(1); y++)
			{
				if (SelectedPixels[x, y] || PreviewSelectedPixels[x, y])
				{
					Canvas.SetOverlayPixel(new Vector2I(x, y) + Offset, new());
				}
			}
		}
	}

	public bool IsSelectedPixel(Vector2I pos)
	{
		pos -= Offset;
		if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
			return false;

		return HasSelection && SelectedPixels[pos.X, pos.Y];
	}
}
