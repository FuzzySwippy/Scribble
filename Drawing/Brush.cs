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

	public static void SampleColor(Vector2I pos) => Global.MainColorInput.Set(Canvas.GetPixel(pos));

	public static void Pencil(Vector2I pos, Color color, bool square)
	{
		if (Size == 1)
		{
			Canvas.SetPixel(pos, color);
			return;
		}

		int sizeAdd = Size / 2;
		for (int x = pos.X - sizeAdd; x <= pos.X + sizeAdd; x++)
		{
			for (int y = pos.Y - sizeAdd; y <= pos.Y + sizeAdd; y++)
			{
				if (square || pos.ToVector2().DistanceTo(new(x, y)) <= (float)Size / 2)
					Canvas.SetPixel(new(x, y), color);
			}
		}
	}

	public static void Line(Vector2 pos1, Vector2 pos2, Color color)
	{
		if (Size == 1)
		{
			while (pos1 != pos2)
			{
				Canvas.SetPixel(pos1.ToVector2I(), color);
				pos1 = pos1.MoveToward(pos2, 1);
			}
			Canvas.SetPixel(pos2.ToVector2I(), color);
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
					Canvas.SetPixel(new(x, y), color);
			}
		}
	}

	public static void LineOfSquares(Vector2 pos1, Vector2 pos2, Color color)
	{
		while (pos1 != pos2)
		{
			Pencil(pos1.ToVector2I(), color, true);
			pos1 = pos1.MoveToward(pos2, 1);
		}
		Pencil(pos2.ToVector2I(), color, true);
	}
}
