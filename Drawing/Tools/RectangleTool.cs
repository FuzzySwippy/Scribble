using System.Linq;
using Godot;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class RectangleTool : DrawingTool
{
	private Vector2I Pos1 { get; set; }
	private bool IsDrawing { get; set; }

	private KeyModifierMask HollowModifier { get; } = KeyModifierMask.MaskAlt;
	private bool Hollow { get; set; } = false;

	public override void Reset() => IsDrawing = false;

	private void UpdateEffectArea()
	{
		Canvas.ClearOverlay(OverlayType.EffectArea);
		Brush.Rectangle(Pos1, MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, Hollow);
	}

	public override void MouseMoveUpdate()
	{
		if (!IsDrawing)
			return;

		UpdateEffectArea();
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (MouseColorInputMap.TryGetValue(
			new(combination.button, combination.modifiers & ~HollowModifier),
			out QuickPencilType value) && !IsDrawing)
		{
			Pos1 = MousePixelPos;
			IsDrawing = true;
			if ((combination.modifiers & HollowModifier) != 0)
				Hollow = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!IsDrawing)
			return;

		if (MouseColorInputMap.TryGetValue(
			new(combination.button, combination.modifiers & ~HollowModifier),
			out QuickPencilType value))
		{
			Brush.Rectangle(Pos1, MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor,
				BrushPixelType.Normal, Hollow);
			IsDrawing = false;
			Hollow = false;
			Canvas.ClearOverlay(OverlayType.EffectArea);
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			IsDrawing = false;
			Canvas.ClearOverlay(OverlayType.EffectArea);
		}
	}
}
