using System;
using Godot;

namespace Scribble;

public class ContextMenuButton
{
	public Action Action { get; private set; }
	Button Button { get; }
	ContextMenu ContextMenu { get; }

	public bool Initialized { get; private set; }

	public ContextMenuButton(Button button, ContextMenu contextMenu)
	{
		ContextMenu = contextMenu;
		Button = button;
		Button.Pressed += () =>
		{
			Action?.Invoke();
			ContextMenu.HideMenu();
		};

		contextMenu.ItemParent.AddChild(Button);
	}

	public void Show(string text, Action action)
	{
		Action = action;
		Button.Text = text;
		ContextMenu.ItemParent.MoveChild(Button, -1);
		Button.Show();
		Initialized = true;
	}

	public void Hide()
	{
		Button.Hide();
		Action = null;
		Button.Text = "";
		Initialized = false;
	}
}