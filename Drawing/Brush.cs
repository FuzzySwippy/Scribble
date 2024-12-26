using System;
using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI;

namespace Scribble.Drawing;

public static class Brush
{
	public const int MaxSize = 128;
	public const int MinSize = 1;

	private static Canvas Canvas => Global.Canvas;

	private static int size = 1;
	public static int Size
	{
		get => size;
		set
		{
			if (size == value)
				return;

			size = value;
			if (size < MinSize)
				size = MinSize;
			else if (size > MaxSize)
				size = MaxSize;

			Status.Set("brush_size", size);
			SizeChanged?.Invoke(size);
		}
	}

	public static event Action<int> SizeChanged;

	private static void SetPixel(Vector2I pos, Color color, BrushPixelType type, HistoryAction historyAction)
	{
		switch (type)
		{
			case BrushPixelType.EffectAreaOverlay:
				Canvas.SetOverlayPixel(pos, color, OverlayType.EffectArea);
				return;
			case BrushPixelType.EffectAreaOverlayAlt:
				Canvas.SetOverlayPixel(pos, color, OverlayType.EffectAreaAlt);
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
					Color oldColor = Canvas.GetPixelNoOpacity(pos);

					if (Canvas.SetPixel(pos, color) && historyAction != null)
						((DrawHistoryAction)historyAction).AddPixelChange(new(pos, oldColor, color));
				}
				return;
		}
	}

	public static void SampleColor(Vector2I pos, bool ignoreLayerOpacity, bool mergeLayers)
	{
		if (mergeLayers)
			Global.QuickPencils.SetColor(ignoreLayerOpacity ?
				Canvas.GetPixelFlattenedNoOpacity(pos) : Canvas.GetPixelFlattened(pos));
		else
			Global.QuickPencils.SetColor(ignoreLayerOpacity ?
				Canvas.GetPixelNoOpacity(pos) : Canvas.GetPixel(pos));
	}

	public static void Dot(Vector2I pos, Color color, BrushPixelType pixelType,
		HistoryAction historyAction) =>
			SetPixel(pos, color, pixelType, historyAction);

	public static List<Vector2I> Pencil(Vector2I pos, Color color, bool square, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		List<Vector2I> pixels = new();

		if (Size == 1)
		{
			SetPixel(pos, color, pixelType, historyAction);
			pixels.Add(pos);
			return pixels;
		}

		int sizeAdd = Size / 2;
		for (int x = pos.X - sizeAdd; x <= pos.X + sizeAdd; x++)
		{
			for (int y = pos.Y - sizeAdd; y <= pos.Y + sizeAdd; y++)
			{
				if (square || pos.ToVector2().DistanceTo(new(x, y)) <= sizeAdd)
				{
					SetPixel(new(x, y), color, pixelType, historyAction);
					pixels.Add(new(x, y));
				}
			}
		}

		return pixels;
	}

	public static void DitherLine(Vector2 pos1, Vector2 pos2, Color color, Color color2,
		HistoryAction historyAction)
	{
		pos1 += new Vector2(0.5f, 0.5f);
		pos2 += new Vector2(0.5f, 0.5f);

		if (Size == 1)
		{
			while (pos1 != pos2)
			{
				SetDitherPixel(pos1.ToVector2I(), color, color2, historyAction);
				pos1 = pos1.MoveToward(pos2, 1);
			}
			SetDitherPixel(pos2.ToVector2I(), color, color2, historyAction);
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
					SetDitherPixel(new(x, y), color, color2, historyAction);
			}
		}
	}

	public static void DitherLineOfSquares(Vector2 pos1, Vector2 pos2, Color color, Color color2,
		HistoryAction historyAction)
	{
		while (pos1 != pos2)
		{
			Dither(pos1.ToVector2I(), color, color2, true, historyAction);
			pos1 = pos1.MoveToward(pos2, 1);
		}
		Dither(pos2.ToVector2I(), color, color2, true, historyAction);
	}

	public static void Dither(Vector2I pos, Color color, Color color2, bool square,
		HistoryAction historyAction)
	{
		if (Size == 1)
		{
			SetDitherPixel(pos, color, color2, historyAction);
			return;
		}

		int sizeAdd = Size / 2;
		for (int x = pos.X - sizeAdd; x <= pos.X + sizeAdd; x++)
		{
			for (int y = pos.Y - sizeAdd; y <= pos.Y + sizeAdd; y++)
			{
				if (square || pos.ToVector2().DistanceTo(new(x, y)) <= sizeAdd)
					SetDitherPixel(new(x, y), color, color2, historyAction);
			}
		}
	}

	private static void SetDitherPixel(Vector2I pos, Color color, Color color2, HistoryAction historyAction)
	{
		if (pos.X % 2 == 0)
			SetPixel(pos, pos.Y % 2 == 0 ? color : color2, BrushPixelType.Normal, historyAction);
		else
			SetPixel(pos, pos.Y % 2 == 0 ? color2 : color, BrushPixelType.Normal, historyAction);
	}

	public static List<Vector2I> Line(Vector2 pos1, Vector2 pos2, Color color, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		List<Vector2I> pixels = new();

		pos1 += new Vector2(0.5f, 0.5f);
		pos2 += new Vector2(0.5f, 0.5f);

		if (Size == 1)
		{
			while (pos1 != pos2)
			{
				SetPixel(pos1.ToVector2I(), color, pixelType, historyAction);
				pixels.Add(pos1.ToVector2I());
				pos1 = pos1.MoveToward(pos2, 1);
			}
			SetPixel(pos2.ToVector2I(), color, pixelType, historyAction);
			pixels.Add(pos2.ToVector2I());
			return pixels;
		}

		float sizeAdd = (float)Size / 2;
		Vector2I point1 = new Vector2(pos1.X < pos2.X ? pos1.X : pos2.X, pos1.Y < pos2.Y ? pos1.Y : pos2.Y).ToVector2I() - Size.ToVector2I();
		Vector2I point2 = new Vector2(pos1.X > pos2.X ? pos1.X : pos2.X, pos1.Y > pos2.Y ? pos1.Y : pos2.Y).ToVector2I() + Size.ToVector2I();

		for (int x = point1.X; x <= point2.X; x++)
		{
			for (int y = point1.Y; y <= point2.Y; y++)
			{
				if (new Vector2(x, y).DistanceToLine(pos1, pos2) <= sizeAdd)
				{
					SetPixel(new(x, y), color, pixelType, historyAction);
					pixels.Add(new(x, y));
				}
			}
		}

		return pixels;
	}

	public static List<Vector2I> LineOfSquares(Vector2 pos1, Vector2 pos2, Color color, BrushPixelType pixelType,
		HistoryAction historyAction)
	{
		List<Vector2I> pixels = new();

		while (pos1 != pos2)
		{
			pixels.AddRange(Pencil(pos1.ToVector2I(), color, true, pixelType, historyAction));
			pos1 = pos1.MoveToward(pos2, 1);
		}
		pixels.AddRange(Pencil(pos2.ToVector2I(), color, true, pixelType, historyAction));

		return pixels;
	}

	private static Color GetFloodPixel(Vector2I pos, bool mergeLayers)
	{
		if (mergeLayers)
			return Canvas.GetPixelFlattenedNoOpacity(pos);
		return Canvas.GetPixelNoOpacity(pos);
	}

	public static void Flood(Vector2I pos, Color color, float threshold, bool diagonal, bool mergeLayers, HistoryAction historyAction,
		BrushPixelType pixelType)
	{
		if (!Canvas.PixelInBounds(pos))
			return;

		Color targetColor = GetFloodPixel(pos, mergeLayers);

		HashSet<Vector2I> visited = new();
		Queue<Vector2I> queue = new();
		queue.Enqueue(pos);

		while (queue.Count > 0)
		{
			Vector2I current = queue.Dequeue();
			visited.Add(current);

			if (!Canvas.PixelInBounds(current) || GetFloodPixel(current, mergeLayers).Delta(targetColor) > threshold)
				continue;

			switch (pixelType)
			{
				case BrushPixelType.Normal:
					if (Canvas.Selection.HasSelection && !Canvas.Selection.IsSelectedPixel(current))
						continue;
					break;
				case BrushPixelType.Selection:
					if (Canvas.Selection.HasSelection && Canvas.Selection.IsSelectedPixel(current))
						continue;
					break;
				case BrushPixelType.Deselection:
					if (!Canvas.Selection.HasSelection || !Canvas.Selection.IsSelectedPixel(current))
						continue;
					break;
				default:
					throw new Exception("Invalid BrushPixelType");
			}

			SetPixel(current, color, pixelType, historyAction);

			Vector2I newPos = new(current.X - 1, current.Y);
			if (!visited.Contains(newPos))
			{
				queue.Enqueue(newPos);
				visited.Add(newPos);
			}

			newPos = new(current.X + 1, current.Y);
			if (!visited.Contains(newPos))
			{
				queue.Enqueue(newPos);
				visited.Add(newPos);
			}

			newPos = new(current.X, current.Y - 1);
			if (!visited.Contains(newPos))
			{
				queue.Enqueue(newPos);
				visited.Add(newPos);
			}

			newPos = new(current.X, current.Y + 1);
			if (!visited.Contains(newPos))
			{
				queue.Enqueue(newPos);
				visited.Add(newPos);
			}

			if (diagonal)
			{
				newPos = new(current.X - 1, current.Y - 1);
				if (!visited.Contains(newPos))
				{
					queue.Enqueue(newPos);
					visited.Add(newPos);
				}

				newPos = new(current.X + 1, current.Y - 1);
				if (!visited.Contains(newPos))
				{
					queue.Enqueue(newPos);
					visited.Add(newPos);
				}

				newPos = new(current.X - 1, current.Y + 1);
				if (!visited.Contains(newPos))
				{
					queue.Enqueue(newPos);
					visited.Add(newPos);
				}

				newPos = new(current.X + 1, current.Y + 1);
				if (!visited.Contains(newPos))
				{
					queue.Enqueue(newPos);
					visited.Add(newPos);
				}
			}
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
