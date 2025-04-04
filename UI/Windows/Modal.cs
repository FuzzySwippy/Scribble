using System;
using System.Linq;
using Godot;
using Scribble.ScribbleLib.Extensions;

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
	private Label textLabel;
	private TextureRect icon;
	private Button[] buttons;
	private ModalButton[] modalButtons;
	private Button confirmButton;
	private Button cancelButton;

	public override void _Ready()
	{
		Node content = GetChild(1).GetChild(0).GetChild(0);

		textLabel = content.GetGrandChild(3).GetChild<Label>(1);
		icon = content.GetGrandChild(3).GetChild<TextureRect>(0);
		buttons = content.GetChild(1).GetChildren().Select(node => node as Button).ToArray();
		buttons.For((button, i) => button.Pressed += () => HandlePressed(i));

		icon.Hide();

		base._Ready();
		Hidden += Cleanup;
		UpdateTargetPosition();
	}

	private void HandlePressed(int index)
	{
		modalButtons[index].Action?.Invoke();
		Hide();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode is Key.Enter or Key.KpEnter)
				confirmButton?.EmitSignal("pressed");
			else if (keyEvent.Keycode is Key.Escape or Key.Backspace)
				cancelButton?.EmitSignal("pressed");
		}
		base._Input(inputEvent);
	}

	private void Cleanup()
	{
		textLabel.Text = "";

		icon.Texture = null;
		icon.Hide();

		UnbindButtons();
		modalButtons = null;

		ClearHiddenEvent();
		Hidden += Cleanup;
	}

	private void BindButtons()
	{
		if (modalButtons.Length > 4)
			throw new Exception("A modal can only have up to 4 buttons.");

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

	private void UnbindButtons()
	{
		for (int i = 0; i < modalButtons.Length; i++)
			buttons[i].Visible = false;
	}

	private static ModalButton[] GenerateButtons(ModalOptions options, params Action[] actions)
	{
		int count = options switch
		{
			ModalOptions.Ok => 1,
			ModalOptions.OkCancel => 2,
			ModalOptions.YesNo => 2,
			ModalOptions.YesNoCancel => 3,
			_ => throw new Exception("Invalid modal options.")
		};

		if (actions != null && actions.Length > count)
			throw new Exception($"Invalid number of actions for option '{options}' requiring {count} actions.");

		if (actions == null)
			actions = new Action[count];
		else if (actions.Length < count)
			actions = actions.Concat(Enumerable.Repeat<Action>(null, count - actions.Length)).ToArray();

		ModalButton[] buttons = options switch
		{
			ModalOptions.Ok => [new("Ok", ModalButtonType.Confirm, actions[0])],
			ModalOptions.OkCancel => [new("Ok", ModalButtonType.Confirm, actions[0]), new("Cancel", ModalButtonType.Cancel, actions[1])],
			ModalOptions.YesNo => [new("Yes", ModalButtonType.Confirm, actions[0]), new("No", ModalButtonType.Cancel, actions[1])],
			ModalOptions.YesNoCancel => [new("Yes", ModalButtonType.Confirm, actions[0]), new("No", ModalButtonType.Cancel, actions[1]), new("Cancel", ModalButtonType.Cancel, actions[2])],
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

