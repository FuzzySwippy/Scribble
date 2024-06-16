using Godot;

namespace Scribble.Drawing;

public struct SelectionChange
{
    public Vector2I Position { get; }
	public bool OldSelected { get; }
	public bool NewSelected { get; }

	public SelectionChange(Vector2I position, bool oldSelected, bool newSelected)
	{
		Position = position;
		OldSelected = oldSelected;
		NewSelected = newSelected;
	}
}
