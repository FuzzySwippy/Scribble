using Godot;
using Scribble.Application;

namespace Scribble.Drawing.Tools.Properties;

public partial class ToolProperties : ScrollContainer
{
	[Export] private DrawingToolType toolType = DrawingToolType.None;
	public DrawingToolType ToolType => toolType;

	public DrawingTool Tool => Global.Canvas.Drawing.DrawingTool;

	public void Enable()
	{
		Show();
		ScrollVertical = 0;
	}

	public void Disable() => Hide();

	public virtual void UpdateProperties() { }
}
