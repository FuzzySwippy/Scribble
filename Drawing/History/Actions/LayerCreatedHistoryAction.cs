using Scribble.Application;

namespace Scribble.Drawing;

public class LayerCreatedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private int Index { get; }

	public LayerCreatedHistoryAction(ulong frameId, int index)
	{
		FrameId = frameId;
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.LayerCreated;
	}

	public override void Undo() =>
		Global.Canvas.SelectFrameAndDeleteLayer(FrameId, Index, false);

	public override void Redo() =>
		Global.Canvas.SelectFrameAndCreateLayer(FrameId, Index, false);
}
