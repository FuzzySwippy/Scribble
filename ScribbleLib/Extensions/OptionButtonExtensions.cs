using System;
using Godot;

namespace Scribble.ScribbleLib.Extensions;

public static class OptionButtonExtensions
{
	public static void AddEnumOptions<T>(this OptionButton optionButton) where T : Enum
	{
		string[] names = Enum.GetNames(typeof(T));
		for (int i = 0; i < names.Length; i++)
			optionButton.AddItem(names[i], i);
	}
}
