using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class LineTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsDrawing { get; set; }

	public override void Reset() => IsDrawing = false;

	public override void Update()
	{
		if (!IsDrawing)
			return;

		Canvas.ClearOverlay();
		Brush.Line(Pos1, MousePixelPos, new(), BrushPixelType.EffectAreaOverlay);
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			if (IsDrawing)
			{
				Brush.Line(Pos1, MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
					BrushPixelType.Normal);
				IsDrawing = false;
				Canvas.ClearOverlay();
			}
			else
			{
				Pos1 = MousePixelPos;
				IsDrawing = true;
			}
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			IsDrawing = false;
			Canvas.ClearOverlay();
		}
	}
}
