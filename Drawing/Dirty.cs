using System;
using Godot;

namespace Scribble.Drawing;

public class Dirty
{
	private readonly Vector2I size;
	private readonly uint[,] dirty;
	private uint dirtyIndex = 1;

	private bool allDirty;
	private bool isDirty;
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

	public void SetAll()
	{
		allDirty = true;
		isDirty = true;
	}

	public bool Get(Vector2I position) => dirty[position.X, position.Y] == dirtyIndex || allDirty;

	public void Clear()
	{
		dirtyIndex++;
		isDirty = false;
		allDirty = false;

		//If the index overflows, reset the array
		if (dirtyIndex == 0)
		{
			for (int x = 0; x < size.X; x++)
				for (int y = 0; y < size.Y; y++)
					dirty[x, y] = 0;

			dirtyIndex = 1;
		}
	}

	/// <summary>
	/// Iterates through all dirty cells and calls the given action on them with their position as the parameter.
	/// </summary>
	/// <param name="action"></param>
	/// <param name="clear">Whether to clear the dirty cells after iterating</param>
	public void Iterate(Action<Vector2I> action, bool clear)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));

		if (!IsDirty)
			return;

		if (allDirty)
		{
			for (int x = 0; x < size.X; x++)
				for (int y = 0; y < size.Y; y++)
					action(new(x, y));
		}
		else
		{
			for (int x = 0; x < size.X; x++)
				for (int y = 0; y < size.Y; y++)
					if (dirty[x, y] == dirtyIndex)
						action(new(x, y));
		}

		if (clear)
			Clear();
	}

	public void IterateRange(Vector2I start, Vector2I end, Action<Vector2I> action)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));

		if (start.X < 0 || start.Y < 0)
			throw new ArgumentOutOfRangeException(nameof(start), "The given range is outside the bounds of the dirty array");

		if (end.X > size.X || end.Y > size.Y)
			throw new ArgumentOutOfRangeException(nameof(end), "The given range is outside the bounds of the dirty array");

		if (start.X > end.X || start.Y > end.Y)
			throw new ArgumentException("The given range is invalid. Start position's X and Y components can not be larger than the end position's", nameof(end));

		if (!IsDirty)
			return;

		if (allDirty)
		{
			for (int x = start.X; x < end.X; x++)
				for (int y = start.Y; y < end.Y; y++)
					action(new(x, y));
		}
		else
		{
			for (int x = start.X; x < end.X; x++)
				for (int y = start.Y; y < end.Y; y++)
					if (dirty[x, y] == dirtyIndex)
						action(new(x, y));
		}
	}
}
