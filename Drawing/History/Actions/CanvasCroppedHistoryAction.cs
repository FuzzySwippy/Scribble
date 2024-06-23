using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class CanvasCroppedHistoryAction : HistoryAction
{
	private Vector2I OldSize { get; }
	private CropType CropType { get; }
	private LayerHistoryData[] LayerHistoryData { get; }

	public CanvasCroppedHistoryAction(Vector2I oldSize, CropType type,
		LayerHistoryData[] layerHistoryData)
	{
		OldSize = oldSize;
		CropType = type;
		LayerHistoryData = layerHistoryData;

		ActionType = HistoryActionType.CropToContent;
		HasChanges = true;
	}

	public override void Undo() =>
		Global.Canvas.ResizeWithLayerData(OldSize, LayerHistoryData);

	public override void Redo() =>
		Global.Canvas.CropToContent(CropType, false);
}
