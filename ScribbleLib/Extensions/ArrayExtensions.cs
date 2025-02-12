using System;
using System.Text;
using Godot;

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
		ArgumentNullException.ThrowIfNull(action);

		for (int x = 0; x < array.GetLength(0); x++)
			for (int y = 0; y < array.GetLength(1); y++)
				action(array[x, y]);
	}

	public static void Print<T>(this T[] array, string title = "")
	{
		StringBuilder sb = new();
		if (!string.IsNullOrEmpty(title))
			sb.Append(title + ": ");

		for (int i = 0; i < array.Length; i++)
		{
			sb.Append(array[i]);
			if (i < array.Length - 1)
				sb.Append(", ");
		}
		GD.Print(sb.ToString());
	}

	public static T[] Append<T>(this T[] array, T[] items)
	{
		Array.Resize(ref array, array.Length + items.Length);
		Array.Copy(items, 0, array, array.Length - items.Length, items.Length);
		return array;
	}

	public static int IndexOf<T>(this T[] array, T[] subArray)
	{
		for (int i = 0; i < array.Length - subArray.Length; i++)
		{
			bool found = true;
			for (int j = 0; j < subArray.Length; j++)
			{
				if (!array[i + j].Equals(subArray[j]))
				{
					found = false;
					break;
				}
			}
			if (found)
				return i;
		}
		return -1;
	}
}