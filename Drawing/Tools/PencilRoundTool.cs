using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class PencilRoundTool : DrawingTool
{
	public override void MouseMoveUpdate()
	{
		foreach (MouseCombination combination in MouseColorInputMap.Keys)
			if (Mouse.IsPressed(combination))
				Brush.Line(MousePixelPos, OldMousePixelPos,
					Artist.GetQuickPencilColor(MouseColorInputMap[combination]).GodotColor,
					BrushPixelType.Normal);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
			Brush.Pencil(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, false);
	}
}
