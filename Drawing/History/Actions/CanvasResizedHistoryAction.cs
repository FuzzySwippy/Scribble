using Godot;
using Scribble.Application;
using Scribble.UI;

namespace Scribble.Drawing;

public class CanvasResizedHistoryAction : HistoryAction
{
	private Vector2I OldSize { get; }
	private Vector2I NewSize { get; }
	private ResizeType ResizeType { get; }
	private LayerHistoryData[] LayerHistoryData { get; }

	public CanvasResizedHistoryAction(Vector2I oldSize, Vector2I newSize, ResizeType resizeType,
		LayerHistoryData[] layerHistoryData)
	{
		OldSize = oldSize;
		NewSize = newSize;
		ResizeType = resizeType;
		LayerHistoryData = layerHistoryData;

		ActionType = HistoryActionType.ResizeCanvas;
		HasChanges = true;
	}

	public override void Undo() =>
		Global.Canvas.ResizeWithLayerData(OldSize, LayerHistoryData);

	public override void Redo() =>
		Global.Canvas.Resize(NewSize, ResizeType, false);
}
