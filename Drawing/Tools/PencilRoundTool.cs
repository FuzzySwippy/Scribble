using Godot;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public class PencilRoundTool : DrawingTool
{
	public override void Update()
	{
		foreach (MouseCombination combination in MouseColorInputMap.Keys)
			if (Mouse.IsPressed(combination))
				Brush.Line(MousePixelPos, OldMousePixelPos,
				Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, false);
	}
}
