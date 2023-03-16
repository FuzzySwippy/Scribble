using Godot;

namespace ScribbleLib.Extensions;
public static class Vector
{
    public static Vector2I ToVector2I(this Vector2 vector) => new((int)vector.X, (int)vector.Y);
    public static Vector2 ToVector2(this Vector2I vector) => new(vector.X, vector.Y);

    public static void Loop(this Vector2 vector, LoopAction2 action) => For.Loop2((int)vector.X, (int)vector.Y, action);
    public static void Loop(this Vector2I vector, LoopAction2 action) => For.Loop2(vector.X, vector.Y, action);

    public static void Loop(this Vector3 vector, LoopAction3 action) => For.Loop3((int)vector.X, (int)vector.Y, (int)vector.Z, action);
    public static void Loop(this Vector3I vector, LoopAction3 action) => For.Loop3(vector.X, vector.Y, vector.Z, action);
}
