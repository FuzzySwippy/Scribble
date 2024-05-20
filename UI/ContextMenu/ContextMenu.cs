using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class ContextMenu : CanvasLayer
{
	private Control menuContainer;
	private Button buttonTemplate;
	private ColorRect separatorTemplate;
	public Control ItemParent { get; private set; }

	private bool hasUninitializedButtons;
	private readonly List<ContextMenuButton> buttons = new();

	private bool hasUninitializedSeparators;
	private readonly List<ContextMenuSeparator> separators = new();

	public override void _Ready()
	{
		Global.ContextMenu = this;

		SetupControls();

		Main.Ready += HideMenu;
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

	private void SetupControls()
	{
		ItemParent = this.GetGrandChild<Control>(3);
		menuContainer = GetChild<Control>(0);

		Control templates = GetChild<Control>(1);
		buttonTemplate = templates.GetChild<Button>(0);
		separatorTemplate = templates.GetChild<ColorRect>(1);
	}

	public static void ShowMenu(Vector2 position, params ContextMenuItem[] items) => Global.ContextMenu.ShowInternal(position, items);
	public static void HideMenu() => Global.ContextMenu.HideInternal();

	private void ShowInternal(Vector2 position, ContextMenuItem[] items)
	{
		if (!items.Any() || items.All(item => item == null))
			throw new System.ArgumentException("ContextMenu items must not be empty");

		HideInternal();

		foreach (ContextMenuItem item in items)
			if (item != null)
				AddItem(item);

		menuContainer.Position = position;
		Show();
	}

	private void HideInternal()
	{
		hasUninitializedButtons = true;
		hasUninitializedSeparators = true;
		Clear();
		Hide();
	}

	private void Clear()
	{
		foreach (ContextMenuButton button in buttons)
			button.Hide();

		foreach (ContextMenuSeparator separator in separators)
			separator.Hide();
	}

	private void AddItem(ContextMenuItem item)
	{
		if (item.IsButton)
			GetButton().Show(item.Text, item.Action);
		else
			GetSeparator().Show();
	}

	private ContextMenuButton GetButton()
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

	private ContextMenuSeparator GetSeparator()
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