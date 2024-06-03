using Godot;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public class FloodTool : DrawingTool
{
	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
			Brush.Flood(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor);
	}
}
