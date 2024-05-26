using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
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
				new("New", () => WindowManager.Show("new_canvas")),
				new("Open", () => FileDialogs.Show(FileDialogType.Open)),
				new("Save", Canvas.SaveToPreviousPath),
				new("Save As", () => FileDialogs.Show(FileDialogType.Save)),
				new("Exit", Main.Quit)
			}
		},
		{
			"help",
			new ContextMenuItem[]
			{
				new("About", () => GD.Print("About"))
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
