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
}
