using Scribble.Application;

namespace Scribble.Drawing;

public class LayerDeletedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private Layer Layer { get; }
	private int Index { get; }

	public LayerDeletedHistoryAction(ulong frameId, Layer layer, int index)
	{
		FrameId = frameId;
		Layer = layer;
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.LayerDeleted;
	}

	public override void Undo() =>
		Global.Canvas.SelectFrameAndRestoreLayer(FrameId, Layer, Index);

	public override void Redo() =>
		Global.Canvas.SelectFrameAndDeleteLayer(FrameId, Index, false);
}
