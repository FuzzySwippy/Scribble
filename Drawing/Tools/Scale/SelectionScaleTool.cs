using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectionScaleTool : DrawingTool
{
	public bool ScalingSelection { get; set; }
	//private Vector2I ScaleStartMousePos { get; set; }
	private float Factor { get; set; }
	private Vector2 Direction { get; set; }
	//private string FactorText => Factor.ToString(".##");

	private MouseButton SelectButton { get; } = MouseButton.Left;

	private Vector2 SelectionCenter => Selection.ScaleRect.GetCenter() + Selection.Offset;
	private float SelectionCenterToPos { get; set; }

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

	public SelectionScaleTool()
	{
		ResetOnSelection = false;
		SelectionTool = true;
	}

	private void RedrawPencilPreview()
	{
		if (!Global.Settings.PencilPreview)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);
			Brush.Dot(MousePixelPos, new(), BrushPixelType.EffectAreaOverlay, null);
			PencilPreviewPixels = [MousePixelPos];
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

		if (ScalingSelection)
		{
			Factor = SelectionCenter.DistanceTo(MousePixelPos) / SelectionCenterToPos;
			Direction = (MousePixelPos - SelectionCenter).Normalized();
			GD.Print($"F: {Factor}; Offset: {Selection.Offset}; Center: {SelectionCenter}; Mouse: {MousePixelPos}; SelectionCenterToPos: {SelectionCenterToPos}");
			Selection.ScaleSelection(Direction, Factor);
			//Status.Set("rotation_angle", FactorText);
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds || !Selection.HasSelection)
			return;

		if (!ScalingSelection && combination.button == SelectButton)
		{
			Selection.TakeScaledColors();
			SelectionCenterToPos = SelectionCenter.DistanceTo(Selection.ScaleRect.Position + Selection.Offset);
			//ScaleStartMousePos = MousePixelPos;
			ScalingSelection = true;
			Factor = SelectionCenter.DistanceTo(MousePixelPos) / SelectionCenterToPos;
			Direction = (MousePixelPos - SelectionCenter).Normalized();
			//Direction = new(1, 1);
			//Status.Set("rotation_angle", FactorText);

			Selection.ScaleSelection(Direction, Factor);
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!ScalingSelection)
			return;

		ScalingSelection = false;
		Selection.CommitScaledColors();
		//Status.Set("rotation_angle", "");
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
		{
			if (ScalingSelection)
				Reset();
			else
				Selection.Clear();
		}
	}

	public override void Reset()
	{
		if (!ScalingSelection)
			return;

		ScalingSelection = false;
		Selection.ScaleSelection(new(1, 1), 1);
		Selection.CommitScaledColors();
		//Status.Set("rotation_angle", "");
	}
}
