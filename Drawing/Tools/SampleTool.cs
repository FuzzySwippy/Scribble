using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SampleTool : DrawingTool
{
	private MouseButton[] SampleColorButtons { get; } = new[] { MouseButton.Left, MouseButton.Right };

	public override void Update()
	{
		Canvas.ClearOverlay(OverlayType.EffectArea);
		if (Global.Settings.PencilPreview)
			Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (SampleColorButtons.Contains(combination.button) && (!Selection.HasSelection || Selection.IsSelectedPixel(MousePixelPos)))
			Brush.SampleColor(MousePixelPos);
	}
}
