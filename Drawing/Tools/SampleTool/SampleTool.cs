using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class SampleTool : DrawingTool
{
	private MouseButton[] SampleColorButtons { get; } =
		[MouseButton.Left, MouseButton.Right, MouseButton.Middle];

	//Tool Properties
	public bool IgnoreLayerOpacity { get; set; }
	public bool MergeLayers { get; set; } = true;

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
			PencilPreviewPixels = [MousePixelPos];
		}
	}


	public override void Selected() =>
		RedrawPencilPreview();

	public override void Deselected() =>
		Canvas.ClearOverlayPixels(OverlayType.EffectArea, PencilPreviewPixels);

	public override void SizeChanged(int size) =>
		RedrawPencilPreview();

	public override void MouseMoveUpdate() =>
		RedrawPencilPreview();

	public override void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (SampleColorButtons.Contains(combination.button) && (!Selection.HasSelection || Selection.IsSelectedPixel(MousePixelPos)))
			Brush.SampleColor(MousePixelPos, IgnoreLayerOpacity, MergeLayers);
	}
}
