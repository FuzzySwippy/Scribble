using Scribble.Application;

namespace Scribble.Drawing;

public class LayerMovedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private int FromIndex { get; }
	private int ToIndex { get; }

	public LayerMovedHistoryAction(ulong frameId, int fromIndex, int toIndex)
	{
		FrameId = frameId;
		FromIndex = fromIndex;
		ToIndex = toIndex;
		HasChanges = true;
		ActionType = HistoryActionType.LayerMoved;
	}

	public override void Undo() =>
		Global.Canvas.SelectFrameAndMoveLayer(FrameId, ToIndex, FromIndex);

	public override void Redo() =>
		Global.Canvas.SelectFrameAndMoveLayer(FrameId, FromIndex, ToIndex);
}
