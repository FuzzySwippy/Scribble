using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class IntegratedMenuBar : Control
{
	private Dictionary<string, ContextMenuItem[]> MenuItems { get; } = new()
	{
		{
			"file",
			new ContextMenuItem[]
			{
				new("New", () => Main.CheckUnsavedChanges(() => WindowManager.Show("new_canvas"))),
				new("Open", () => Main.CheckUnsavedChanges(() => FileDialogs.Show(FileDialogType.Open))),
				new("Save", "Ctrl+S", () => Try.Catch(() => Canvas.SaveToPreviousPath(), null)),
				new("Save As", () => FileDialogs.Show(FileDialogType.Save)),
				new("Export", () => FileDialogs.Show(FileDialogType.Export)),
				new("Export Scaled", () => WindowManager.Show("export_scaled")),
				new(),
				new("Settings", () => WindowManager.Show("settings")),
				new(),
				new("Exit", () => Main.CheckUnsavedChanges(Main.Quit))
			}
		},
		{
			"view",
			new ContextMenuItem[]
			{
				new("Grid", () => WindowManager.Show("grid"))
			}
		},
		{
			"image",
			new ContextMenuItem[]
			{
				new("Crop To Content", () => Global.Canvas.CropToContent(CropType.All)),
				new("Crop To Content Vertically", () => Global.Canvas.CropToContent(CropType.Vertical)),
				new("Crop To Content Horizontally", () => Global.Canvas.CropToContent(CropType.Horizontal)),
			}
		},
		{
			"help",
			new ContextMenuItem[]
			{
				new("About", "", () => WindowManager.Show("about"))
			}
		}
	};

	private bool MenuOpen { get; set; }

	public override void _Ready()
	{
		Main.Ready += () => Global.ContextMenu.Closed += () => MenuOpen = false;

		SetupButtons();
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
}
