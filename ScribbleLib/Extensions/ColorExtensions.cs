using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorExtensions
{
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
}
