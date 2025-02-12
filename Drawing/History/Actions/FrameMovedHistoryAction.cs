using Scribble.Application;

namespace Scribble.Drawing;

public class FrameMovedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private int FromIndex { get; }
	private int ToIndex { get; }

	public FrameMovedHistoryAction(ulong frameId, int fromIndex, int toIndex)
	{
		FrameId = frameId;
		FromIndex = fromIndex;
		ToIndex = toIndex;
		HasChanges = true;
		ActionType = HistoryActionType.FrameMoved;
	}

	public override void Undo() =>
		Global.Canvas.Animation.MoveFrame(FrameId, FromIndex);

	public override void Redo() =>
		Global.Canvas.Animation.MoveFrame(FrameId, ToIndex);
}
