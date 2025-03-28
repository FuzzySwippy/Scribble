using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectionRotateTool : DrawingTool
{
	public bool RotatingSelection { get; set; }
	private Vector2I RotateStartMousePos { get; set; }
	private float Angle { get; set; }
	private string TextAngle => (Angle < 0 ? 360 + Angle : Angle).ToString(".##");

	private MouseButton SelectButton { get; } = MouseButton.Left;

	//Properties
	public bool InterpolateEmptyPixels { get; set; } = true;
	public bool IgnoreEmptyColors { get; set; } = true;

	public SelectionRotateTool()
	{
		ResetOnSelection = false;
		SelectionTool = true;
	}

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

	private Vector2 Center => (Selection.RotationCenter + Selection.Offset).ToVector2();

	private void RedrawPencilPreview()
	{
		if (!Global.Settings.PencilPreview)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);
			Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
			PencilPreviewPixels = new List<Vector2I> { MousePixelPos };
		}
	}


	public override void Selected() =>
		RedrawPencilPreview();

	public override void Deselected() =>
		Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);

	public override void SizeChanged(int size) =>
		RedrawPencilPreview();

	public override void MouseMoveUpdate()
	{
		RedrawPencilPreview();

		if (RotatingSelection)
		{
			Angle = Center.AngleBetween3Points(RotateStartMousePos, MousePixelPos);
			Selection.RotateSelection(Angle, InterpolateEmptyPixels, IgnoreEmptyColors);
			Status.Set("rotation_angle", TextAngle);
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds || !Selection.HasSelection)
			return;

		if (!RotatingSelection && combination.button == SelectButton)
		{
			RotateStartMousePos = MousePixelPos;
			Selection.TakeRotatedColors();
			RotatingSelection = true;
			Angle = 0;
			Status.Set("rotation_angle", TextAngle);
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!RotatingSelection)
			return;

		RotatingSelection = false;
		Selection.CommitRotatedColors();
		Status.Set("rotation_angle", "");
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			if (RotatingSelection)
				Reset();
			else
				Selection.Clear();
		}
	}

	public override void Reset()
	{
		if (!RotatingSelection)
			return;

		RotatingSelection = false;
		Selection.RotateSelection(0, InterpolateEmptyPixels, IgnoreEmptyColors);
		Selection.CommitRotatedColors();
		Status.Set("rotation_angle", "");
	}
}
