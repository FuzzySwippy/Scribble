using System;

namespace Scribble.ScribbleLib.Extensions;

public static class ArrayExtensions
{
	public static T[] Foreach<T>(this T[] array, Action<T> action)
	{
		for (int i = 0; i < array.Length; i++)
			action(array[i]);
		return array;
	}

	public static T[] For<T>(this T[] array, Action<T, int> action)
	{
		for (int i = 0; i < array.Length; i++)
			action(array[i], i);
		return array;
	}

	public static int Count<T>(this T[,] array, Func<T, bool> predicate = null)
	{
		int count = 0;
		for (int x = 0; x < array.GetLength(0); x++)
			for (int y = 0; y < array.GetLength(1); y++)
				if (predicate?.Invoke(array[x, y]) ?? true)
					count++;
		return count;
	}

	public static void ForEach<T>(this T[,] array, Action<T> action)
	{
		if (action == null)
			throw new ArgumentNullException(nameof(action));

		for (int x = 0; x < array.GetLength(0); x++)
			for (int y = 0; y < array.GetLength(1); y++)
				action(array[x, y]);
	}
}