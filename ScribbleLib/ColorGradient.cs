using Godot;

namespace Scribble.ScribbleLib;

public static class ColorGradient
{
	public static Color Linear(Vector2 start, Vector2 end, Vector2 point, Color startColor, Color endColor) =>
		startColor.Lerp(endColor, Mathf.Clamp(AComponentOfLength(start, end, point) / start.DistanceTo(end), 0f, 1f));

	private static float AComponentOfLength(Vector2 start, Vector2 end, Vector2 point)
	{
		float x = start.DistanceTo(end);
		float y = start.DistanceTo(point);
		float z = end.DistanceTo(point);

		return (x * x + y * y - z * z) / (2 * x);
	}

	public static Color Radial(Vector2 start, Vector2 end, Vector2 point, Color startColor, Color endColor) =>
		startColor.Lerp(endColor, Mathf.Clamp(start.DistanceTo(point) / start.DistanceTo(end), 0f, 1f));
}
