using Godot;

namespace Scribble;

public class ContextMenuSeparator
{
    ColorRect Separator { get; }
    ContextMenu ContextMenu { get; }

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
