using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SelectionMoveTool : DrawingTool
{
	public bool MovingSelection { get; set; }
	private Vector2I MoveStartMousePos { get; set; }
	private Vector2I MoveStartOffset { get; set; }
	private bool NoSelectionMove { get; set; }

	private MouseButton CancelButton { get; } = MouseButton.Right;
	private MouseButton SelectButton { get; } = MouseButton.Left;

	public SelectionMoveTool()
	{
		ResetOnSelection = false;
		SelectionTool = true;
	}

	//Pencil Preview
	private List<Vector2I> PencilPreviewPixels { get; set; }

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

		if (MovingSelection)
			Selection.Offset = MousePixelPos - MoveStartMousePos;
	}

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (combination.button == CancelButton)
		{
			Reset();
			return;
		}

		if (!MovingSelection && combination.button == SelectButton)
		{
			if (!Selection.HasSelection)
			{
				Selection.SetPixelWithHistory(MousePixelPos);
				NoSelectionMove = true;
			}
			else
				NoSelectionMove = false;

			MoveStartMousePos = MousePixelPos - Selection.Offset;
			MoveStartOffset = Selection.Offset;
			Selection.TakeSelectedColors();
			Selection.Update();
			MovingSelection = true;
		}
	}

	public override void MouseUp(MouseCombination combination, Vector2 position)
	{
		if (!MovingSelection)
			return;

		MovingSelection = false;
		Selection.CommitSelectedColors();

		if (NoSelectionMove || !Selection.InBounds)
			Selection.Clear();
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Reset();
	}

	public override void Reset()
	{
		if (MovingSelection)
		{
			MovingSelection = false;
			Selection.Offset = MoveStartOffset;
			Selection.CommitSelectedColors();
		}

		Canvas.ClearOverlay(OverlayType.EffectArea);
		Canvas.Selection.Clear();
	}
}
