namespace Scribble.ScribbleLib.Extensions;

public static class Math
{
	public static bool InRangeEx(this float value, float min, float max) => value > min && value < max;
	public static bool InRangeIn(this float value, float min, float max) => value >= min && value <= max;

	public static float PositiveAngle(this float angle) => angle < 0 ? 360 + angle : angle;
}
