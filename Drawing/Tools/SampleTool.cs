using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;

namespace Scribble.Drawing.Tools;

public class SampleTool : DrawingTool
{
	private MouseButton[] SampleColorButtons { get; } = new[] { MouseButton.Left, MouseButton.Right };

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (SampleColorButtons.Contains(combination.button))
			Brush.SampleColor(MousePixelPos);
	}
}
