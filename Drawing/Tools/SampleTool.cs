using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SampleTool : DrawingTool
{
	private MouseButton[] SampleColorButtons { get; } = new[] { MouseButton.Left, MouseButton.Right };

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (SampleColorButtons.Contains(combination.button))
			Brush.SampleColor(MousePixelPos);
	}
}
