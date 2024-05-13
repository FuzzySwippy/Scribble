using System.Collections;
using System.Collections.Generic;

namespace ScribbleLib;

/// <summary>
/// A list that only allows unique items.
/// </summary>
public class UniqueList<T> : IEnumerable<T>
{
	private readonly List<T> items = new();
	private readonly HashSet<T> set = new();

	public T this[int index] => items[index];


	public int Count => items.Count;

	public bool Add(T item)
	{
		if (set.Add(item))
		{
			items.Add(item);
			return true;
		}
		return false;
	}

	public bool Remove(T item)
	{
		if (set.Remove(item))
		{
			items.Remove(item);
			return true;
		}
		return false;
	}

	public bool Contains(T item) => set.Contains(item);

	public void Clear()
	{
		items.Clear();
		set.Clear();
	}

	public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
