using System.Collections.Generic;
using Godot;
using ScribbleLib;

namespace Scribble;

public partial class ContextMenu : CanvasLayer
{
    Control menuContainer;
    Button buttonTemplate;
    ColorRect separatorTemplate;
    public Control ItemParent { get; private set; }

    bool hasUninitializedButtons;
    readonly List<ContextMenuButton> buttons = new();

    bool hasUninitializedSeparators;
    readonly List<ContextMenuSeparator> separators = new();

    public override void _Ready()
    {
        Global.ContextMenu = this;

        SetupControls();
        Visible = false;
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (!Visible)
            return;

        if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
            HideMenu();

        if (inputEvent is InputEventMouseButton buttonEvent && buttonEvent.Pressed && !menuContainer.GetRect().HasPoint(buttonEvent.Position))
            HideMenu();
    }

    void SetupControls()
    {
        ItemParent = this.GetGrandChild<Control>(3);
        menuContainer = GetChild<Control>(0);

        Control templates = GetChild<Control>(1);
        buttonTemplate = templates.GetChild<Button>(0);
        separatorTemplate = templates.GetChild<ColorRect>(1);
    }

    public static void ShowMenu(Vector2 position, params ContextMenuItem[] items) => Global.ContextMenu.ShowInternal(position, items);
    public static void HideMenu() => Global.ContextMenu.HideInternal();

    void ShowInternal(Vector2 position, ContextMenuItem[] items)
    {
        if (items.Length == 0)
            throw new System.ArgumentException("ContextMenu items must not be empty");

        HideInternal();
        
        foreach (ContextMenuItem item in items)
            AddItem(item);

        menuContainer.Position = position;
        Show();
    }

    void HideInternal()
    {
        hasUninitializedButtons = true;
        hasUninitializedSeparators = true;
        Clear();
        Hide();
    }

    void Clear()
    {
        foreach (ContextMenuButton button in buttons)
            button.Hide();

        foreach (ContextMenuSeparator separator in separators)
            separator.Hide();
    }

    void AddItem(ContextMenuItem item)
    {
        if (item.IsButton)
            GetButton().Show(item.Text, item.Action);
        else
            GetSeparator().Show();
    }

    ContextMenuButton GetButton()
    {
        //Search for an uninitialized button
        if (hasUninitializedButtons)
        {
            for (int i = 0; i < buttons.Count; i++)
                if (!buttons[i].Initialized)
                    return buttons[i];

            hasUninitializedButtons = false;
        }

        //Create a new button
        ContextMenuButton button = new((Button)buttonTemplate.Duplicate(), this);
        buttons.Add(button);
        return button;
    }

    ContextMenuSeparator GetSeparator()
    {
        //Search for an uninitialized separator
        if (hasUninitializedSeparators)
        {
            for (int i = 0; i < separators.Count; i++)
                if (!separators[i].Initialized)
                    return separators[i];

            hasUninitializedSeparators = false;
        }

        //Create a new separator
        ContextMenuSeparator separator = new((ColorRect)separatorTemplate.Duplicate(), this);
        separators.Add(separator);
        return separator;
    }
}