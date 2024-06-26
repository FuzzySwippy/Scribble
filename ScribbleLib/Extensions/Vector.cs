using Godot;

namespace Scribble.ScribbleLib.Extensions;
public static class Vector
{
	//Conversion
	public static Vector2I ToVector2I(this Vector2 vector) => new((int)vector.X, (int)vector.Y);
	public static Vector2 ToVector2(this Vector2I vector) => new(vector.X, vector.Y);
	public static Vector2 ToVector2(this float value) => new(value, value);
	public static Vector2 ToVector2(this int value) => new(value, value);
	public static Vector2I ToVector2I(this int value) => new(value, value);

	//Distance
	public static float DistanceToLineDividend(this Vector2 point, Vector2 line1, Vector2 line2) => Mathf.Abs(((line2.X - line1.X) * (line1.Y - point.Y)) - ((line1.X - point.X) * (line2.Y - line1.Y)));
	public static float DistanceToLineDivisor(Vector2 line1, Vector2 line2) => Mathf.Sqrt(((line2.X - line1.X) * (line2.X - line1.X)) + ((line2.Y - line1.Y) * (line2.Y - line1.Y)));
	public static float DistanceToLine(this Vector2 point, Vector2 line1, Vector2 line2)
	{
		Vector2 AB = new(line2.X - line1.X, line2.Y - line1.Y);
		Vector2 BE = new(point.X - line2.X, point.Y - line2.Y);
		Vector2 AE = new(point.X - line1.X, point.Y - line1.Y);

		if (AB.Y * BE.Y + AB.X * BE.X > 0)
			return point.DistanceTo(line2);
		else if (AB.Y * AE.Y + AB.X * AE.X < 0)
			return point.DistanceTo(line1);
		else
			return Mathf.Abs(AB.Y * AE.X - AB.X * AE.Y) / Mathf.Sqrt(AB.Y * AB.Y + AB.X * AB.X);
	}

	public static void Loop(this Vector2 vector, LoopAction2 action) => For.Loop2((int)vector.X, (int)vector.Y, action);
	public static void Loop(this Vector2I vector, LoopAction2 action) => For.Loop2(vector.X, vector.Y, action);

	public static void Loop(this Vector3 vector, LoopAction3 action) => For.Loop3((int)vector.X, (int)vector.Y, (int)vector.Z, action);
	public static void Loop(this Vector3I vector, LoopAction3 action) => For.Loop3(vector.X, vector.Y, vector.Z, action);
}
