using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorTools
{
	public static Color GrayscaleColor(this float value, float alpha = 1) => new(value, value, value, alpha);

	public static Color SetR(this Color color, float r) => new(r, color.G, color.B, color.A);
	public static Color SetG(this Color color, float g) => new(color.R, g, color.B, color.A);
	public static Color SetB(this Color color, float b) => new(color.R, color.G, b, color.A);
	public static Color SetA(this Color color, float a) => new(color.R, color.G, color.B, a);

	public static Color MultiplyA(this Color color, float mult) => new(color.R, color.G, color.B, color.A * mult);

	public static byte[] ToByteArray(this Color[,] colors, float opacity = 1)
	{
		Vector2I size = new(colors.GetLength(0), colors.GetLength(1));
		byte[] output = new byte[size.X * size.Y * 4];
		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++)
			{
				int index = (y * size.X + x) * 4;
				Color color = colors[x, y].MultiplyA(opacity);

				output[index] = (byte)color.R8;
				output[index + 1] = (byte)color.G8;
				output[index + 2] = (byte)color.B8;
				output[index + 3] = (byte)color.A8;
			}
		}

		return output;
	}

	public static Color[,] GetColorsFromImage(this Image image)
	{
		Vector2I size = new(image.GetWidth(), image.GetHeight());
		Color[,] colors = new Color[size.X, size.Y];
		for (int x = 0; x < size.X; x++)
			for (int y = 0; y < size.Y; y++)
				colors[x, y] = image.GetPixel(x, y);

		return colors;
	}
}
