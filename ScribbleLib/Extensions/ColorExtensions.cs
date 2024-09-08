using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorExtensions
{
	public static float Delta(this Color color, Color targetColor)
	{
		float r = color.R * color.A;
		float g = color.G * color.A;
		float b = color.B * color.A;
		float r2 = targetColor.R * targetColor.A;
		float g2 = targetColor.G * targetColor.A;
		float b2 = targetColor.B * targetColor.A;
		return (Mathf.Abs(r - r2) + Mathf.Abs(g - g2) + Mathf.Abs(b - b2)) / 3;
	}
}
