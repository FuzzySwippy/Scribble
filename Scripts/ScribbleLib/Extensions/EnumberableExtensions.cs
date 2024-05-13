
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScribbleLib;

public static class EnumberableExtensions
{
	public static IEnumerable<T> Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		foreach (T item in enumerable)
			action(item);
		return enumerable;
	}

	public static bool InRange<T>(this IEnumerable<T> enumerable, int index) => index >= 0 && index < enumerable.Count();

	public static T Get<T>(this IEnumerable<T> enumerable, int index) => enumerable.InRange(index) ? enumerable.ElementAt(index) : default;
}
