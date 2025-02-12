using Scribble.Application;

namespace Scribble.Drawing;

public class LayerCreatedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private Layer Layer { get; }
	private int Index { get; }

	public LayerCreatedHistoryAction(ulong frameId, Layer layer, int index)
	{
		FrameId = frameId;
		Layer = layer;
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.LayerCreated;
	}

	public override void Undo() =>
		Global.Canvas.SelectFrameAndDeleteLayer(FrameId, Layer.Id, false);

	public override void Redo() =>
		Global.Canvas.SelectFrameAndRestoreLayer(FrameId, Layer, Index);
}
