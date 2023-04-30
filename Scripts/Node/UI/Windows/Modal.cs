using System;
using System.Linq;
using Godot;
using ScribbleLib;

namespace Scribble;

public partial class Modal : Window
{
    Label textLabel;
    TextureRect icon;
    Button[] buttons;
    ModalButton[] modalButtons;
    Action[] buttonActions;

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
            if (keyEvent.Keycode == Key.Enter)
                confirmButton?.EmitSignal("pressed");
            else if (keyEvent.Keycode == Key.Escape)
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

    public Modal Show(string text, Texture2D iconTexture, ModalButton[] buttons)
    {
        textLabel.Text = text;
        icon.Texture = iconTexture;
        icon.Show();
        modalButtons = buttons;
        BindButtons();

        Show();
        return this;
    }

    public Modal Show(string text, ModalButton[] buttons)
    {
        textLabel.Text = text;
        modalButtons = buttons;
        BindButtons();

        Show();
        return this;
    }
}

