using System;
using System.Collections.Generic;

namespace ScribbleLib;

public static class ArrayExtensions
{
    public static T[] Foreach<T>(this T[] array, Action<T> action)
    {
        for (int i = 0; i < array.Length; i++)
            action(array[i]);
        return array;
    }

    public static T[] For<T>(this T[] array, Action<T,int> action)
    {
        for (int i = 0; i < array.Length; i++)
            action(array[i], i);
        return array;
    }

    public static IEnumerable<T> Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T item in enumerable)
            action(item);
        return enumerable;
    }
}