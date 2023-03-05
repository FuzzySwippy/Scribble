using Godot;

namespace ScribbleLib.Extensions;
public static class Vector
{
    public static Vector2I ToVector2I(this Vector2 vector) => new((int)vector.X, (int)vector.Y);
    public static Vector2 ToVector2(this Vector2I vector) => new(vector.X, vector.Y);
}
