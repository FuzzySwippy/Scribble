using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI;

public partial class IntegratedMenuBar : Control
{
	private Dictionary<string, ContextMenuItem[]> MenuItems { get; set; }

	private bool MenuOpen { get; set; }

	public override void _Ready()
	{
		Main.Ready += MainReady;
		Keyboard.KeyDown += KeyDown;

		SetupButtons();
	}

	private void MainReady()
	{
		Global.ContextMenu.Closed += () => MenuOpen = false;
		MenuItems = new()
		{
			{
				"file",
				new ContextMenuItem[]
				{
					new("New", new(Key.N, KeyModifierMask.MaskCtrl), () => Main.CheckUnsavedChanges(() => WindowManager.Show("new_canvas"))),
					new("Open", new(Key.O, KeyModifierMask.MaskCtrl), () => Main.CheckUnsavedChanges(() => FileDialogs.Show(FileDialogType.Open))),
					new("Save", new(Key.S, KeyModifierMask.MaskCtrl), () => Try.Catch(() => Canvas.SaveToPreviousPath(), null)),
					new("Save As", new(Key.S, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift), () => FileDialogs.Show(FileDialogType.Save)),
					new("Export", new(Key.E, KeyModifierMask.MaskCtrl), () => FileDialogs.Show(FileDialogType.Export)),
					new("Export Scaled", new(Key.N, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift), () => WindowManager.Show("export_scaled")),
					new(),
					new("Settings", new(Key.P, KeyModifierMask.MaskCtrl), () => WindowManager.Show("settings")),
					new(),
					new("Exit", new(Key.Q, KeyModifierMask.MaskCtrl), () => Main.CheckUnsavedChanges(Main.Quit))
				}
			},
			{
				"edit",
				new ContextMenuItem[]
				{
					new("Undo", new(Key.Z, KeyModifierMask.MaskCtrl), Global.Canvas.Undo),
					new("Redo", new(Key.Z, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift), Global.Canvas.Redo),
					new(),
					new("Cut", new(Key.X, KeyModifierMask.MaskCtrl), Global.Canvas.Cut),
					new("Copy", new(Key.C, KeyModifierMask.MaskCtrl), Global.Canvas.Copy),
					new("Paste", new(Key.V, KeyModifierMask.MaskCtrl), Global.Canvas.Paste),
				}
			},
			{
				"view",
				new ContextMenuItem[]
				{
					new("Grid", new(Key.G, KeyModifierMask.MaskCtrl), () => WindowManager.Show("grid")),
					new("Animation Timeline", new(Key.A, KeyModifierMask.MaskCtrl), Global.AnimationTimeline.Toggle),
					new("Animation Settings", new(Key.A, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift), () => WindowManager.Get("animation").Show()),
				}
			},
			{
				"image",
				new ContextMenuItem[]
				{
					new("Crop To Content", new(Key.T, KeyModifierMask.MaskCtrl), () => Global.Canvas.CropToContent(CropType.All)),
					new("Crop To Content Vertically", new(Key.T, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskAlt), () => Global.Canvas.CropToContent(CropType.Vertical)),
					new("Crop To Content Horizontally", new(Key.T, KeyModifierMask.MaskCtrl | KeyModifierMask.MaskShift), () => Global.Canvas.CropToContent(CropType.Horizontal)),
				}
			},
			{
				"help",
				new ContextMenuItem[]
				{
					new("About", new(Key.Slash, KeyModifierMask.MaskCtrl), () => WindowManager.Show("about"))
				}
			}
		};
	}

	private void SetupButtons()
	{
		Control buttonContainer = this.GetGrandChild<Control>(3);

		foreach (Node node in buttonContainer.GetChildren())
		{
			Button button = (Button)node;
			string entryName = button.Text.ToLower();

			button.Pressed += () => OpenMenu(button, entryName);

			button.MouseEntered += () =>
			{
				if (MenuOpen)
					OpenMenu(button, entryName);
			};
		}
	}

	private void OpenMenu(Button button, string entryName)
	{
		ContextMenu.ShowMenu(button.GlobalPosition + new Vector2(0, button.Size.Y), MenuItems[entryName]);
		MenuOpen = true;
	}

	private void KeyDown(KeyCombination combination)
	{
		if (Global.WindowManager.WindowOpen)
			return;

		foreach (ContextMenuItem[] items in MenuItems.Values)
		{
			foreach (ContextMenuItem item in items)
			{
				if (item.Shortcut == combination)
				{
					ContextMenu.HideMenu();
					item.Action?.Invoke();
					return;
				}
			}
		}
	}
}
