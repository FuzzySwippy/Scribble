using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ContextMenu : PopupMenu
{
    ContextMenuOption[] options;

    public override void _Ready()
    {
        Global.ContextMenu = this;
        IndexPressed += OnIndexPressed;
    }

    void OnIndexPressed(long index)
    {
        if (index < 0 || index >= options.Length)
            throw new System.Exception($"Index {index} is out of range for options array of length {options.Length}.");

        options[index]?.Action();
        Hide();
    }

    public static void Show(Vector2 position, params ContextMenuOption[] options) => Global.ContextMenu.InternalShow(position, options);

    void InternalShow(Vector2 position, ContextMenuOption[] options)
    {
        if (Visible)
            Hide();

        this.options = options;
        Clear();
        foreach (ContextMenuOption option in options)
            AddItem(option.Text);

        Popup(new(position.ToVector2I(), Vector2I.Zero));
    }
}