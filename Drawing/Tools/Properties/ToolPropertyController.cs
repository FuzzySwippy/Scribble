using Godot;
using Godot.Collections;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Drawing.Tools.Properties;

public partial class ToolPropertyController : Control
{
	private System.Collections.Generic.Dictionary<DrawingToolType, ToolProperties> PropertyControls
	{ get; } = new();

	private Label TitleLabel { get; set; }

	public override void _Ready()
	{
		Global.Canvas.Initialized += OnCanvasInitialized;
		TitleLabel = this.GetGrandChild<Label>(3);
		GetPropertyControls();
	}

	private void OnCanvasInitialized()
	{
		Global.Canvas.Drawing.ToolTypeChanged += OnToolTypeChanged;
		OnToolTypeChanged(Global.Canvas.Drawing.ToolType);
	}

	private void GetPropertyControls()
	{
		Control parent = this.GetGrandChild<Control>(2);
		Array<Node> childNodes = parent.GetChildren();

		foreach (Node node in childNodes)
			if (node is ToolProperties toolProperties)
				PropertyControls.Add(toolProperties.ToolType, toolProperties);
	}

	private void OnToolTypeChanged(DrawingToolType toolType)
	{
		int visibleCount = 0;
		foreach (ToolProperties properties in PropertyControls.Values)
		{
			properties.Visible = properties.ToolType == toolType;
			if (properties.Visible)
				visibleCount++;
		}

		if (visibleCount == 0)
			PropertyControls[DrawingToolType.None].Visible = true;
		else
			PropertyControls[toolType].UpdateProperties();

		TitleLabel.Text = $"{toolType.ToString().AddSpacesBeforeCapitalLetters()} Properties";
	}
}
