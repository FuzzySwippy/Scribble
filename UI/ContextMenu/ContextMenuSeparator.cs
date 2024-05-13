using Godot;

namespace Scribble.UI;

public class ContextMenuSeparator
{
	private ColorRect Separator { get; }
	private ContextMenu ContextMenu { get; }

	public bool Initialized { get; private set; }

	public ContextMenuSeparator(ColorRect separator, ContextMenu contextMenu)
	{
		Separator = separator;
		ContextMenu = contextMenu;
	}

	public void Show()
	{
		Separator.Show();
		ContextMenu.ItemParent.MoveChild(Separator, -1);
		Initialized = true;
	}
	public void Hide()
	{
		Separator.Hide();
		Initialized = false;
	}
}
