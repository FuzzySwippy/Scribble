using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorExtensions
{
	private static Color EmptyColor { get; } = new Color(0);

	public static float Delta(this Color color, Color targetColor)
	{
		float r = color.R;
		float g = color.G;
		float b = color.B;
		float a = color.A;
		float r2 = targetColor.R;
		float g2 = targetColor.G;
		float b2 = targetColor.B;
		float a2 = targetColor.A;
		return (Mathf.Abs(r - r2) + Mathf.Abs(g - g2) + Mathf.Abs(b - b2) + Mathf.Abs(a - a2) * 5) / 8;
	}

	public static Color Average(this Color color, params Color[] colors)
	{
		float r = color.R;
		float g = color.G;
		float b = color.B;
		float a = color.A;

		foreach (Color c in colors)
		{
			r += c.R;
			g += c.G;
			b += c.B;
			a += c.A;
		}

		int count = colors.Length + 1;
		return new Color(r / count, g / count, b / count, a / count);
	}

	public static Color AverageIgnoreEmpty(this Color color, params Color[] colors)
	{
		float r = color.R;
		float g = color.G;
		float b = color.B;
		float a = color.A;
		int count = colors.Length + 1;

		if (color == EmptyColor)
			count--;

		foreach (Color c in colors)
		{
			if (c == EmptyColor)
			{
				count--;
				continue;
			}

			r += c.R;
			g += c.G;
			b += c.B;
			a += c.A;
		}

		if (count == 0)
			return EmptyColor;
		return new Color(r / count, g / count, b / count, a / count);
	}
}
