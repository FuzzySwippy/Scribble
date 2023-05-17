using System.Collections.Generic;
using System;
using Godot;

namespace Scribble;

public partial class WindowManager : Control
{
	[Export] public float TransitionTime { get; set; }

	readonly Dictionary<string, Window> windows = new();
	readonly List<Modal> modals = new();

	public override void _Ready()
	{
		Global.WindowManager = this;
		RegisterWindows();
	}

	void RegisterWindows()
	{
		foreach (Node node in GetChildren())
			if (node is Window window && node is not Modal && !string.IsNullOrWhiteSpace(window.KeyName))
				windows.Add(window.KeyName, window);
	}

	public static Window Get(string name) => Global.WindowManager.windows.TryGetValue(name, out Window window) ? window : null;
	public static Window Show(string name) => (Get(name) ?? throw new Exception($"Window with name '{name}' not found.")).Show();

	Modal GetModal()
	{
		//Search for unused modal
		foreach (Modal modal in modals)
			if (!modal.Visible)
				return modal;

		//Create new modal
		Modal newModal = Global.ModalPrefab.Instantiate<Modal>();
		modals.Add(newModal);
		AddChild(newModal);
		return newModal;
	}

	public static Modal ShowModal(string text, ModalButton[] buttons) => Global.WindowManager.GetModal().Show(text, buttons);
	public static Modal ShowModal(string text, Texture2D icon, ModalButton[] buttons) => Global.WindowManager.GetModal().Show(text, icon, buttons);

	public static Modal ShowModal(string text, ModalOptions options, params Action[] actions) => Global.WindowManager.GetModal().Show(text, options, actions);
	public static Modal ShowModal(string text, Texture2D icon, ModalOptions options, params Action[] actions) => Global.WindowManager.GetModal().Show(text, icon, options, actions);
}
