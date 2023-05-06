using Godot;

namespace Scribble;

public partial class ContextMenu : PopupMenu
{
    ContextMenuOption[] options;

    public void Show(Vector2I position, ContextMenuOption[] options)
    {
        this.options = options;
        Clear();
        foreach (ContextMenuOption option in options)
            AddItem(option.Text);

        Popup(new(position, Vector2I.Zero));
    }

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
}