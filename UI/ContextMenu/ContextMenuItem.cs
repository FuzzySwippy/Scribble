using System;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI;

public class ContextMenuItem
{
	public bool IsButton { get; }
	public string Text { get; }
	public KeyCombination? Shortcut { get; }
	public Action Action { get; }

	/// <summary>
	/// Creates a new context menu Button item.
	/// </summary>
	/// <param name="text">Button text</param>
	/// <param name="action">Button press action</param>
	public ContextMenuItem(string text, KeyCombination? shortcut, Action action)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ArgumentException("Text cannot be null or whitespace", nameof(text));

		Text = text;
		Shortcut = shortcut;
		Action = action;
		IsButton = true;
	}

	public ContextMenuItem(string text, Action action) : this(text, null, action) { }

	/// <summary>
	/// Creates a new context menu Separator item.
	/// </summary>
	public ContextMenuItem() => IsButton = false;
}