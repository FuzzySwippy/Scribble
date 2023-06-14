using Godot;

namespace Scribble.Drawing;

public class Dirty
{
	readonly Vector2I size;
	readonly uint[,] dirty;
	uint dirtyIndex = 1;

	bool isDirty;
	public bool IsDirty => isDirty;

	public Dirty(Vector2I size)
	{
		this.size = size;
		dirty = new uint[size.X, size.Y];
	}

	public void Set(Vector2I position)
	{
		dirty[position.X, position.Y] = dirtyIndex;
		isDirty = true;
	}

	public bool Get(Vector2I position) => dirty[position.X, position.Y] == dirtyIndex;

	public void Clear()
	{
		dirtyIndex++;
		isDirty = false;

		//If the index overflows, reset the array
		if (dirtyIndex == 0)
		{
			for (int x = 0; x < size.X; x++)
				for (int y = 0; y < size.Y; y++)
					dirty[x, y] = 0;

			dirtyIndex = 1;
		}
	}
}
