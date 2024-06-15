using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI;

namespace Scribble.Drawing;

public static class Brush
{
	private static Canvas Canvas => Global.Canvas;

	private static int size = 1;
	public static int Size
	{
		get => size;
		set
		{
			size = value;
			if (size < 1)
				size = 1;
			else if (size > 100)
				size = 100;

			Status.Set("brush_size", size);
		}
	}

	private static void SetPixel(Vector2I pos, Color color, BrushPixelType type, HistoryAction historyAction)
	{
		switch (type)
		{
			case BrushPixelType.EffectAreaOverlay:
				Canvas.SetOverlayPixel(pos, color, OverlayType.EffectArea);
				return;
			case BrushPixelType.Selection:
				if (!Canvas.Selection.TryGetPixel(pos, out bool current) || current)
					return;

				Canvas.Selection.SetPixel(pos, true);
				if (historyAction != null)
					((SelectionChangedHistoryAction)historyAction).AddSelectionChange(new(pos, current, true));
				break;
			case BrushPixelType.Deselection:
				if (!Canvas.Selection.TryGetPixel(pos, out bool current2) || !current2)
					return;

				Canvas.Selection.SetPixel(pos, false);
				if (historyAction != null)
					((SelectionChangedHistoryAction)historyAction).AddSelectionChange(new(pos, current2, false));
				return;
			default:
				if (!Canvas.Selection.HasSelection || Canvas.Selection.IsSelectedPixel(pos))
				{
					Color oldColor = Canvas.GetPixel(pos);

					if (Canvas.SetPixel(pos, color) && historyAction != null)
						((DrawHistoryAction)historyAction).AddPixelChange(new(pos, oldColor, color));
				}
				return;
		}
	}

	public static void SampleColor(Vector2I pos) => Global.MainColorInput.Set(Canvas.GetPixel(pos));

	public static void Pencil(Vector2I pos, Color color, bool square, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		if (Size == 1)
		{
			SetPixel(pos, color, pixelType, historyAction);
			return;
		}

		int sizeAdd = Size / 2;
		for (int x = pos.X - sizeAdd; x <= pos.X + sizeAdd; x++)
		{
			for (int y = pos.Y - sizeAdd; y <= pos.Y + sizeAdd; y++)
			{
				if (square || pos.ToVector2().DistanceTo(new(x, y)) <= (float)Size / 2)
					SetPixel(new(x, y), color, pixelType, historyAction);
			}
		}
	}

	public static void Line(Vector2 pos1, Vector2 pos2, Color color, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		if (Size == 1)
		{
			while (pos1 != pos2)
			{
				SetPixel(pos1.ToVector2I(), color, pixelType, historyAction);
				pos1 = pos1.MoveToward(pos2, 1);
			}
			SetPixel(pos2.ToVector2I(), color, pixelType, historyAction);
			return;
		}

		float sizeAdd = (float)Size / 2;
		Vector2I point1 = new Vector2(pos1.X < pos2.X ? pos1.X : pos2.X, pos1.Y < pos2.Y ? pos1.Y : pos2.Y).ToVector2I() - Size.ToVector2I();
		Vector2I point2 = new Vector2(pos1.X > pos2.X ? pos1.X : pos2.X, pos1.Y > pos2.Y ? pos1.Y : pos2.Y).ToVector2I() + Size.ToVector2I();

		for (int x = point1.X; x <= point2.X; x++)
		{
			for (int y = point1.Y; y <= point2.Y; y++)
			{
				if (new Vector2(x, y).DistanceToLine(pos1, pos2) <= sizeAdd)
					SetPixel(new(x, y), color, pixelType, historyAction);
			}
		}
	}

	public static void LineOfSquares(Vector2 pos1, Vector2 pos2, Color color, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		while (pos1 != pos2)
		{
			Pencil(pos1.ToVector2I(), color, true, pixelType, historyAction);
			pos1 = pos1.MoveToward(pos2, 1);
		}
		Pencil(pos2.ToVector2I(), color, true, pixelType, historyAction);
	}

	public static void Flood(Vector2I pos, Color color, HistoryAction historyAction)
	{
		if (!Canvas.PixelInBounds(pos))
			return;

		Color targetColor = Canvas.GetPixel(pos);
		if (targetColor == color)
			return;

		Queue<Vector2I> queue = new();
		queue.Enqueue(pos);

		while (queue.Count > 0)
		{
			Vector2I current = queue.Dequeue();
			if (!Canvas.PixelInBounds(current) || Canvas.GetPixel(current) != targetColor)
				continue;

			SetPixel(current, color, BrushPixelType.Normal, historyAction);
			queue.Enqueue(new(current.X - 1, current.Y));
			queue.Enqueue(new(current.X + 1, current.Y));
			queue.Enqueue(new(current.X, current.Y - 1));
			queue.Enqueue(new(current.X, current.Y + 1));
		}
	}

	public static void Rectangle(Vector2I pos1, Vector2I pos2, Color color,
		BrushPixelType pixelType, bool hollow, HistoryAction historyAction)
	{
		int x1 = pos1.X < pos2.X ? pos1.X : pos2.X;
		int x2 = pos1.X > pos2.X ? pos1.X : pos2.X;
		int y1 = pos1.Y < pos2.Y ? pos1.Y : pos2.Y;
		int y2 = pos1.Y > pos2.Y ? pos1.Y : pos2.Y;

		for (int x = x1; x <= x2; x++)
			for (int y = y1; y <= y2; y++)
				if (!hollow || x == x1 || x == x2 || y == y1 || y == y2)
					SetPixel(new(x, y), color, pixelType, historyAction);
	}
}
