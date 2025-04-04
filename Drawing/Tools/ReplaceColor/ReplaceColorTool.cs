using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing.Tools;

public class ReplaceColorTool : DrawingTool
{
	//Properties
	public bool AllLayers { get; set; } = false;
	public bool AllFrames { get; set; } = false;
	public bool IgnoreOpacity { get; set; } = false;

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

		if (MouseColorInputMap.TryGetValue(combination, out QuickPencilType value))
		{
			ReplaceColorHistoryAction historyAction = new();
			Brush.ReplaceColor(MousePixelPos, Artist.GetQuickPencilColor(value).GodotColor, AllLayers, AllFrames, IgnoreOpacity, historyAction);
			Canvas.History.AddAction(historyAction);
		}
	}

	public override void KeyDown(KeyCombination combination)
	{
		if (CancelKeys.Contains(combination.key))
			Selection.Clear();
	}
}
