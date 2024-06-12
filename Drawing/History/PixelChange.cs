using Godot;

namespace Scribble.Drawing;

public class PixelChange
{
	public Vector2I Position { get; }
    public Color OldColor { get; }
	public Color NewColor { get; }

	public PixelChange(Vector2I position, Color oldColor, Color newColor)
	{
		Position = position;
		OldColor = oldColor;
		NewColor = newColor;
	}
}
