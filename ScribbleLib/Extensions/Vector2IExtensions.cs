using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class Vector2IExtensions
{
    public static Vector2I Min(this Vector2I a, Vector2I b) => new(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
	public static Vector2I Max(this Vector2I a, Vector2I b) => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));
}
