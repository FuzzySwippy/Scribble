using Godot;
using Scribble.Drawing;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorArray
{
	public static Color[,] CropToContent(this Color[,] colors, CropType type, out Rect2I bounds)
	{
		int minX = colors.GetLength(0);
		int minY = colors.GetLength(1);
		int maxX = 0;
		int maxY = 0;

		for (int x = 0; x < colors.GetLength(0); x++)
		{
			for (int y = 0; y < colors.GetLength(1); y++)
			{
				if (colors[x, y].A > 0)
				{
					minX = Mathf.Min(minX, x);
					minY = Mathf.Min(minY, y);
					maxX = Mathf.Max(maxX, x);
					maxY = Mathf.Max(maxY, y);
				}
			}
		}

		if (minX > maxX || minY > maxY)
		{
			bounds = new Rect2I(0, 0, 0, 0);
			return new Color[0, 0];
		}

		bounds = type switch
		{
			CropType.Vertical => new Rect2I(minX, 0, maxX - minX + 1, colors.GetLength(1)),
			CropType.Horizontal => new Rect2I(0, minY, colors.GetLength(0), maxY - minY + 1),
			_ => new Rect2I(minX, minY, maxX - minX + 1, maxY - minY + 1),
		};

		return colors.CropToBounds(bounds);
	}

	public static Color[,] CropToBounds(this Color[,] colors, Rect2I bounds)
	{
		Color[,] cropped = new Color[bounds.Size.X, bounds.Size.Y];
		for (int x = 0; x < bounds.Size.X; x++)
			for (int y = 0; y < bounds.Size.Y; y++)
				cropped[x, y] = colors[x + bounds.Position.X, y + bounds.Position.Y];

		return cropped;
	}
}
