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
	private Vector2 Factor { get; set; }
	private string FactorText => $"({Factor.X:0.##}, {Factor.Y:0.##})";
	private Vector2 Direction { get; set; }

	private MouseButton SelectButton { get; } = MouseButton.Left;

	private Vector2 SelectionCenter => Selection.ScaleRect.GetCenter() + Selection.Offset;
	private Vector2 SelectionPos => Selection.ScaleRect.Position + Selection.Offset;
	private Vector2 SelectionEnd => Selection.ScaleRect.End + Selection.Offset;

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

	private float SingleDirectionThreshold => 0.5f;

	//Properties
	public bool UseForceFactor { get; set; } = false;
	public Vector2 ForceFactor { get; set; } = new(1, 1);

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

	private Vector2 CalculateDirection()
	{
		Vector2 dir = (MousePixelPos - SelectionCenter).Normalized();
		dir = new(Mathf.Abs(dir.X) > SingleDirectionThreshold ? dir.X : 0, Mathf.Abs(dir.Y) > SingleDirectionThreshold ? dir.Y : 0);
		dir = new(dir.X == 0 ? 0 : dir.X > 0 ? 1 : -1, dir.Y == 0 ? 0 : dir.Y > 0 ? 1 : -1);
		return dir;
	}

	private Vector2 CalculateFactor()
	{
		if (UseForceFactor)
			return ForceFactor;

		float factorX = Direction.X == 0 ? 1 : Direction.X > 0 ? (MousePixelPos.X - SelectionPos.X) / Selection.ScaleRect.Size.X : (MousePixelPos.X - SelectionEnd.X) / -Selection.ScaleRect.Size.X;
		float factorY = Direction.Y == 0 ? 1 : Direction.Y > 0 ? (MousePixelPos.Y - SelectionPos.Y) / Selection.ScaleRect.Size.Y : (MousePixelPos.Y - SelectionEnd.Y) / -Selection.ScaleRect.Size.Y;
		return new(factorX, factorY);
	}

	public override void MouseMoveUpdate()
	{
		RedrawPencilPreview();

		if (ScalingSelection)
		{
			Factor = CalculateFactor();
			Selection.ScaleSelection(Direction, Factor);
			Status.Set("scale_factor", FactorText);
		}
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds || !Selection.HasSelection)
			return;

		if (!ScalingSelection && combination.button == SelectButton)
		{
			Selection.TakeScaledColors();
			ScalingSelection = true;
			Direction = CalculateDirection();
			Factor = CalculateFactor();
			Status.Set("scale_factor", FactorText);

			Selection.ScaleSelection(Direction, Factor);
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!ScalingSelection)
			return;

		ScalingSelection = false;
		Selection.CommitScaledColors();
		Status.Set("scale_factor", "");
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
		Selection.ScaleSelection(new(1, 1), new(1, 1));
		Selection.CommitScaledColors();
		Status.Set("scale_factor", "");
	}
}
