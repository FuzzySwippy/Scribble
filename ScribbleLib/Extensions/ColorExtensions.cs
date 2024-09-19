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
}
