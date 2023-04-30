using System;

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
}