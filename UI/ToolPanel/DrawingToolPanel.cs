using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI;

public partial class DrawingToolPanel : Node
{
	private ToolButton[] ToolButtons { get; set; }
	private DrawingToolType ToolType
	{
		get => Global.Canvas.Drawing.ToolType;
		set => Global.Canvas.Drawing.ToolType = value;
	}

	public override void _Ready()
	{
		Global.DrawingToolPanel = this;

		Control buttonContainer = this.GetGrandChild(2).GetChild<Control>(1);
		Node[] buttons = buttonContainer.GetChildren().ToArray();
		ToolButtons = new ToolButton[buttons.Length];
		for (int i = 0; i < buttons.Length; i++)
		{
			ToolButtons[i] = buttons[i].GetChild<ToolButton>(0);
			ToolButtons[i].Init(this);
			if (ToolButtons[i].ToolType == Global.DefaultToolType)
				ToolButtons[i].Select();
		}
	}

	public void Select(DrawingToolType toolType)
	{
		ToolType = toolType;
		DeselectAll();

		//Select button having the current tool type
		foreach (ToolButton button in ToolButtons)
		{
			if (button.ToolType == toolType)
			{
				button.Select();
				break;
			}
		}
	}

	private void DeselectAll()
	{
		foreach (ToolButton button in ToolButtons)
			button?.Deselect();
	}
}
