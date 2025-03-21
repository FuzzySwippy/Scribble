using System.Collections.Generic;

namespace Scribble.ScribbleLib.Extensions;

public static class HashSet
{
	public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
	{
		foreach (T item in collection)
			hashSet.Add(item);
	}
}
