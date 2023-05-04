using System;
using System.Linq;
using Godot;
using ScribbleLib;

namespace Scribble;

public enum ModalOptions
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel
}

public partial class Modal : Window
{
    Label textLabel;
    TextureRect icon;
    Button[] buttons;
    ModalButton[] modalButtons;

    Button confirmButton;
    Button cancelButton;

    public override void _Ready()
    {
        Node content = GetChild(1).GetChild(0).GetChild(0);

        textLabel = content.GetChild(0).GetChild<Label>(1);
        icon = content.GetChild(0).GetChild<TextureRect>(0);
        buttons = content.GetChild(1).GetChildren().Select(node => node as Button).ToArray();
        buttons.For((button, i) => button.Pressed += () => HandlePressed(i));

        icon.Hide();

        base._Ready();
        Hidden += Cleanup;
        UpdateTargetPosition();
    }

    void HandlePressed(int index)
    { 
        modalButtons[index].Action?.Invoke();
        Hide();
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Enter || keyEvent.Keycode == Key.KpEnter)
                confirmButton?.EmitSignal("pressed");
            else if (keyEvent.Keycode == Key.Escape || keyEvent.Keycode == Key.Backspace)
                cancelButton?.EmitSignal("pressed");
        }
        base._Input(inputEvent);
    }

    void Cleanup()
    {
        textLabel.Text = "";

        icon.Texture = null;
        icon.Hide();

        UnbindButtons();
        modalButtons = null;
    }

    void BindButtons()
    {
        if (modalButtons.Length > 4)
            throw new Exception("Modal can only have up to 4 buttons.");

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < modalButtons.Length)
            {
                buttons[i].Text = modalButtons[i].Text;
                buttons[i].Visible = true;

                if (modalButtons[i].Type == ModalButtonType.Confirm)
                    confirmButton = buttons[i];
                else if (modalButtons[i].Type == ModalButtonType.Cancel)
                    cancelButton = buttons[i];
            }
            else
                buttons[i].Visible = false;
        }
    }

    void UnbindButtons()
    {
        for (int i = 0; i < modalButtons.Length; i++)
            buttons[i].Visible = false;
    }

    ModalButton[] GenerateButtons(ModalOptions options, params Action[] actions)
    {
        int count = options switch
        {
            ModalOptions.Ok => 1,
            ModalOptions.OkCancel => 2,
            ModalOptions.YesNo => 2,
            ModalOptions.YesNoCancel => 3,
            _ => throw new Exception("Invalid modal options.")
        };

        if (actions.Length != count)
            throw new Exception($"Invalid number of actions for option '{options}' requiring {count} actions.");

        ModalButton[] buttons = options switch
        {
            ModalOptions.Ok => new ModalButton[] { new ModalButton("Ok", ModalButtonType.Confirm, actions[0]) },
            ModalOptions.OkCancel => new ModalButton[] { new ModalButton("Ok", ModalButtonType.Confirm, actions[0]), new ModalButton("Cancel", ModalButtonType.Cancel, actions[1]) },
            ModalOptions.YesNo => new ModalButton[] { new ModalButton("Yes", ModalButtonType.Confirm, actions[0]), new ModalButton("No", ModalButtonType.Cancel, actions[1]) },
            ModalOptions.YesNoCancel => new ModalButton[] { new ModalButton("Yes", ModalButtonType.Confirm, actions[0]), new ModalButton("No", ModalButtonType.Cancel, actions[1]), new ModalButton("Cancel", ModalButtonType.Cancel, actions[2]) },
            _ => throw new Exception("Invalid modal options.")
        };

        return buttons;
    }

    public Modal Show(string text, Texture2D iconTexture, ModalButton[] buttons)
    {
        if (buttons == null || buttons.Length == 0)
            throw new Exception("Modal must have at least one button.");

        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Modal must have text.");
            
        textLabel.Text = text;

        if (iconTexture != null)
        {
            icon.Texture = iconTexture;
            icon.Show();
        }

        modalButtons = buttons;
        BindButtons();

        Show();
        return this;
    }

    public Modal Show(string text, ModalButton[] buttons) => Show(text, null, buttons);

    public Modal Show(string text, Texture2D iconTexture, ModalOptions options, params Action[] actions) => Show(text, iconTexture, GenerateButtons(options, actions));

    public Modal Show(string text, ModalOptions options, params Action[] actions) => Show(text, GenerateButtons(options, actions));
}

