using System;
using Godot;

namespace Scribble.UI;

public class ContextMenuButton
{
	private PanelContainer ButtonPanel { get; }
	private Button Button { get; }
	private Label TextLabel { get; }
	private Label ShortcutLabel { get; }

	private ContextMenu ContextMenu { get; }
	public Action Action { get; private set; }

	public bool Initialized { get; private set; }

	public ContextMenuButton(PanelContainer buttonPanel, ContextMenu contextMenu)
	{
		ButtonPanel = buttonPanel;
		Button = buttonPanel.GetChild<Button>(0);
		TextLabel = buttonPanel.GetChild(1).GetChild(0).GetChild<Label>(0);
		ShortcutLabel = buttonPanel.GetChild(1).GetChild(0).GetChild<Label>(2);

		ContextMenu = contextMenu;

		Button.Pressed += () =>
		{
			Action?.Invoke();
			ContextMenu.HideMenu();
		};

		contextMenu.ItemParent.AddChild(ButtonPanel);
	}

	public void Show(ContextMenuItem item)
	{
		Action = item.Action;
		TextLabel.Text = item.Text;
		ShortcutLabel.Text = string.IsNullOrWhiteSpace(item.Shortcut) ? "" : $"({item.Shortcut})";
		ContextMenu.ItemParent.MoveChild(ButtonPanel, -1);

		ButtonPanel.Show();
		Initialized = true;
	}

	public void Hide()
	{
		ButtonPanel.Hide();
		Action = null;
		TextLabel.Text = "";
		ShortcutLabel.Text = "";
		Initialized = false;
	}
}