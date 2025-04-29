using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Drawing.Tools;
using Scribble.Drawing.Tools.Gradient;
using Scribble.ScribbleLib;
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

	private static BlendMode blendMode = BlendMode.Overwrite;
	public static BlendMode BlendMode
	{
		get => blendMode;
		set
		{
			if (blendMode == value)
				return;

			blendMode = value;
			BlendModeChanged?.Invoke(value);
		}
	}

	public static event Action<BlendMode> BlendModeChanged;

	private static void SetPixel(Vector2I pos, Color color, BrushPixelType type, HistoryAction historyAction)
	{
		switch (type)
		{
			case BrushPixelType.Preview:
				if (!Canvas.Selection.HasSelection || Canvas.Selection.IsSelectedPixel(pos))
					Canvas.SetOverlayPixel(pos, color, OverlayType.Preview);
				return;
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

					if (Canvas.BlendPixel(pos, color, BlendMode) && historyAction != null)
						((DrawHistoryAction)historyAction).AddPixelChange(new(pos, oldColor, Canvas.GetPixelNoOpacity(pos)));
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

	#region Pencil
	public static List<Vector2I> Pencil(Vector2I pos, Color color, ShapeType shapeType, BrushPixelType pixelType,
		HistoryAction historyAction, HashSet<Vector2I> ignorePositions = null)
	{
		if (Size == 1)
		{
			SetPixel(pos, color, pixelType, historyAction);
			return [pos];
		}

		return shapeType switch
		{
			ShapeType.Round => PencilRound(pos, color, pixelType, historyAction, ignorePositions),
			ShapeType.Square => PencilSquare(pos, color, pixelType, historyAction, ignorePositions),
			_ => throw new Exception("Invalid ShapeType"),
		};
	}

	private static List<Vector2I> PencilRound(Vector2I pos, Color color, BrushPixelType pixelType, HistoryAction historyAction, HashSet<Vector2I> ignorePositions = null)
	{
		List<Vector2I> pixels = [];

		Vector2 centerPos = pos.ToVector2();
		float halfSize = (float)Size / 2;
		for (int x = 0; x <= Size; x++)
		{
			for (int y = 0; y <= Size; y++)
			{
				Vector2I pos2 = new(x + pos.X - (int)halfSize, y + pos.Y - (int)halfSize);
				if (centerPos.DistanceTo(pos2) <= halfSize && (ignorePositions == null || !ignorePositions.Contains(pos2)))
				{
					SetPixel(pos2, color, pixelType, historyAction);
					pixels.Add(pos2);
				}
			}
		}

		return pixels;
	}

	private static List<Vector2I> PencilSquare(Vector2I pos, Color color, BrushPixelType pixelType, HistoryAction historyAction, HashSet<Vector2I> ignorePositions = null)
	{
		List<Vector2I> pixels = [];

		int halfSize = Size / 2;
		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				Vector2I pos2 = new(x + pos.X - halfSize, y + pos.Y - halfSize);
				if (ignorePositions == null || !ignorePositions.Contains(pos2))
				{
					SetPixel(pos2, color, pixelType, historyAction);
					pixels.Add(pos2);
				}
			}
		}

		return pixels;
	}
	#endregion

	#region Dither
	public static void DitherLine(Vector2 pos1, Vector2 pos2, Color color, Color color2, ShapeType shapeType, HistoryAction historyAction)
	{
		pos1 += new Vector2(0.5f, 0.5f);
		pos2 += new Vector2(0.5f, 0.5f);

		while (pos1 != pos2)
		{
			Dither(pos1.ToVector2I(), color, color2, shapeType, historyAction);
			pos1 = pos1.MoveToward(pos2, 1);
		}

		Dither(pos2.ToVector2I(), color, color2, shapeType, historyAction);
	}

	public static void Dither(Vector2I pos, Color color, Color color2, ShapeType shapeType, HistoryAction historyAction)
	{
		if (Size == 1)
		{
			SetDitherPixel(pos, color, color2, historyAction);
			return;
		}

		switch (shapeType)
		{
			case ShapeType.Round:
				DitherRound(pos, color, color2, historyAction);
				break;
			case ShapeType.Square:
				DitherSquare(pos, color, color2, historyAction);
				break;
			default:
				throw new Exception("Invalid ShapeType");
		}
	}

	private static void DitherRound(Vector2I pos, Color color, Color color2, HistoryAction historyAction)
	{
		float halfSize = (float)Size / 2;
		for (int x = 0; x <= Size; x++)
		{
			for (int y = 0; y <= Size; y++)
			{
				Vector2I pos2 = new(x + pos.X - (int)halfSize, y + pos.Y - (int)halfSize);
				if (pos.ToVector2().DistanceTo(pos2) <= halfSize)
					SetDitherPixel(pos2, color, color2, historyAction);
			}
		}
	}

	private static void DitherSquare(Vector2I pos, Color color, Color color2, HistoryAction historyAction)
	{
		int halfSize = Size / 2;
		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				Vector2I pos2 = new(x + pos.X - halfSize, y + pos.Y - halfSize);
				SetDitherPixel(pos2, color, color2, historyAction);
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
	#endregion

	public static List<Vector2I> Line(Vector2 pos1, Vector2 pos2, Color color, ShapeType shapeType, BrushPixelType pixelType, HistoryAction historyAction)
	{
		HashSet<Vector2I> pixels = [];

		pos1 += new Vector2(0.5f, 0.5f);
		pos2 += new Vector2(0.5f, 0.5f);

		while (pos1 != pos2)
		{
			pixels.AddRange(Pencil(pos1.ToVector2I(), color, shapeType, pixelType, historyAction, pixels));
			pos1 = pos1.MoveToward(pos2, 1);
		}

		pixels.AddRange(Pencil(pos2.ToVector2I(), color, shapeType, pixelType, historyAction, pixels));
		return pixels.ToList();
	}

	public static List<Vector2I> Gradient(Vector2I pos1, Vector2I pos2, Color color, Color color2, GradientType gradientType, BrushPixelType pixelType, HistoryAction historyAction)
	{
		HashSet<Vector2I> pixels = [];

		for (int x = 0; x < Canvas.Size.X; x++)
		{
			for (int y = 0; y < Canvas.Size.Y; y++)
			{
				Vector2I pos = new(x, y);

				switch (gradientType)
				{
					case GradientType.Linear:
						SetPixel(pos, ColorGradient.Linear(pos1, pos2, pos, color, color2), pixelType, historyAction);
						break;
					case GradientType.Radial:
						SetPixel(pos, ColorGradient.Radial(pos1, pos2, pos, color, color2), pixelType, historyAction);
						break;
					default:
						throw new Exception("Invalid GradientType");
				}
				pixels.Add(pos);
			}
		}
		return pixels.ToList();
	}

	private static Color GetFloodPixel(Vector2I pos, bool mergeLayers)
	{
		if (mergeLayers)
			return Canvas.GetPixelFlattenedNoOpacity(pos);
		return Canvas.GetPixelNoOpacity(pos);
	}

	public static void Flood(Vector2I pos, Color color, float threshold, bool diagonal, bool mergeLayers, HistoryAction historyAction, BrushPixelType pixelType)
	{
		if (!Canvas.PixelInBounds(pos))
			return;

		Color targetColor = GetFloodPixel(pos, mergeLayers);

		HashSet<Vector2I> visited = [];
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

	#region ReplaceColor
	private static Color GetReplaceColorPixel(Vector2I pos, bool ignoreOpacity)
	{
		if (ignoreOpacity)
			return Canvas.GetPixelFlattenedNoOpacity(pos);
		return Canvas.GetPixelFlattened(pos);
	}

	private static void ReplaceColorAtPos(Vector2I pos, Color targetColor, Color color, Frame frame, Layer layer, Func<Vector2I, Layer, Color> getPixelFunc, HistoryAction historyAction)
	{
		Color oldColor = getPixelFunc(pos, layer);
		if (oldColor == color || oldColor != targetColor)
			return;

		if (Canvas.SetPixelInLayer(pos, color, layer) && historyAction != null)
			((ReplaceColorHistoryAction)historyAction).AddPixelChange(new(pos, oldColor, color), frame.Id, layer.Id);
	}

	private static void ReplaceColorInFrameLayer(Color targetColor, Color color, Frame frame, Layer layer, bool ignoreOpacity, HistoryAction historyAction)
	{
		for (int x = 0; x < Canvas.Size.X; x++)
		{
			for (int y = 0; y < Canvas.Size.Y; y++)
			{
				Vector2I pos = new(x, y);
				if (Canvas.Selection.HasSelection && !Canvas.Selection.IsSelectedPixel(pos))
					continue;

				if (ignoreOpacity)
					ReplaceColorAtPos(pos, targetColor, color, frame, layer, Canvas.GetPixelNoOpacityInLayer, historyAction);
				else
					ReplaceColorAtPos(pos, targetColor, color, frame, layer, Canvas.GetPixelInLayer, historyAction);
			}
		}
	}

	public static void ReplaceColor(Vector2I pos, Color color, bool allLayers, bool allFrames, bool ignoreOpacity, HistoryAction historyAction)
	{
		if (!Canvas.PixelInBounds(pos))
			return;

		if (allFrames)
			allLayers = true;

		Color targetColor = GetReplaceColorPixel(pos, ignoreOpacity);

		if (allFrames)
		{
			foreach (Frame frame in Canvas.Animation.Frames)
			{
				foreach (Layer layer in frame.Layers)
				{
					ReplaceColorInFrameLayer(targetColor, color, frame, layer, ignoreOpacity, historyAction);
					layer.PreviewNeedsUpdate = true;
				}
				frame.PreviewNeedsUpdate = true;
			}
		}
		else if (allLayers)
		{
			foreach (Layer layer in Canvas.CurrentFrame.Layers)
			{
				ReplaceColorInFrameLayer(targetColor, color, Canvas.CurrentFrame, layer, ignoreOpacity, historyAction);
				layer.PreviewNeedsUpdate = true;
			}
		}
		else
			ReplaceColorInFrameLayer(targetColor, color, Canvas.CurrentFrame, Canvas.CurrentLayer, ignoreOpacity, historyAction);
	}
	#endregion

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
