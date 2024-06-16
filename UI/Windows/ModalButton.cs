using Godot;
using System;

namespace Scribble;

public enum ModalButtonType
{
	Confirm,
	Cancel,
	Normal
}

public class ModalButton
{
	public ModalButtonType Type { get; }
	public string Text { get; }
	public Action Action { get; }

	public ModalButtonStyles Styles { get; set; }

	public ModalButton(string text, ModalButtonType type, Action action)
	{
		Type = type;
		Text = text;
		Action = action;
	}

	public ModalButton(string text, ModalButtonType type, Action action, ModalButtonStyles styles) : this(text, type, action) => Styles = styles;
}

public class ModalButtonStyles
{
	public StyleBox Normal { get; }
	public StyleBox Hover { get; }
	public StyleBox Pressed { get; }

	public ModalButtonStyles(StyleBox normal, StyleBox hover, StyleBox pressed)
	{
		Normal = normal;
		Hover = hover;
		Pressed = pressed;
	}
}