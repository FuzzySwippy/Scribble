using Godot;
using ColorBlender;
using ColorBlender.Models.ColorModels.RGB;

namespace Scribble.ScribbleLib.Extensions;

public static class ColorExtensions
{
	private static ColorBlenderService ColorBlender { get; } = new ColorBlenderService();
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

	public static URGB ToURGB(this Color color) =>
		new((decimal)color.R, (decimal)color.G, (decimal)color.B, (decimal)color.A);

	public static Color ToGodotColor(this URGB rgb) =>
		new((float)rgb.R, (float)rgb.G, (float)rgb.B, (float)rgb.A);

	#region BlendModes
	public static Color Normal(this Color bottomColor, Color topColor)
	{
		if (topColor.A == 1)
			return topColor;
		return bottomColor.Blend(topColor);
	}

	public static Color Add(this Color bottomColor, Color topColor) =>
		new(
			Mathf.Min(bottomColor.R + topColor.R, 1),
			Mathf.Min(bottomColor.G + topColor.G, 1),
			Mathf.Min(bottomColor.B + topColor.B, 1),
			Mathf.Min(bottomColor.A + topColor.A, 1)
		);

	public static Color Subtract(this Color bottomColor, Color topColor) =>
		new(
			Mathf.Max(bottomColor.R - topColor.R, 0),
			Mathf.Max(bottomColor.G - topColor.G, 0),
			Mathf.Max(bottomColor.B - topColor.B, 0),
			Mathf.Max(bottomColor.A - topColor.A, 0)
		);

	public static Color Divide(this Color bottomColor, Color topColor) =>
		new(
			topColor.R == 0 ? 0 : bottomColor.R / topColor.R,
			topColor.G == 0 ? 0 : bottomColor.G / topColor.G,
			topColor.B == 0 ? 0 : bottomColor.B / topColor.B,
			topColor.A == 0 ? 0 : bottomColor.A / topColor.A
		);

	public static Color Multiply(this Color bottomColor, Color topColor) =>
		ColorBlender.Multiply(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Overlay(this Color bottomColor, Color topColor) =>
		ColorBlender.Overlay(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Screen(this Color bottomColor, Color topColor) =>
		ColorBlender.Screen(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Color(this Color bottomColor, Color topColor) =>
		ColorBlender.Color(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color ColorBurn(this Color bottomColor, Color topColor) =>
		ColorBlender.ColorBurn(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color ColorDodge(this Color bottomColor, Color topColor) =>
		ColorBlender.ColorDodge(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Darken(this Color bottomColor, Color topColor) =>
		ColorBlender.Darken(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Difference(this Color bottomColor, Color topColor) =>
	ColorBlender.Difference(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Exclusion(this Color bottomColor, Color topColor) =>
		ColorBlender.Exclusion(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color HardLight(this Color bottomColor, Color topColor) =>
		ColorBlender.HardLight(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Hue(this Color bottomColor, Color topColor) =>
		ColorBlender.Hue(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Lighten(this Color bottomColor, Color topColor) =>
		ColorBlender.Lighten(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Luminosity(this Color bottomColor, Color topColor) =>
		ColorBlender.Luminosity(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color Saturation(this Color bottomColor, Color topColor) =>
		ColorBlender.Saturation(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();

	public static Color SoftLight(this Color bottomColor, Color topColor) =>
		ColorBlender.SoftLight(bottomColor.ToURGB(), topColor.ToURGB()).ToUrgb().ToGodotColor();
	#endregion
}
