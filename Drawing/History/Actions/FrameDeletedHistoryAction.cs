using Scribble.Application;

namespace Scribble.Drawing;

public class FrameDeletedHistoryAction : HistoryAction
{
	private Frame Frame { get; }
	private int Index { get; }

	public FrameDeletedHistoryAction(Frame frame, int index)
	{
		Frame = frame;
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.FrameDeleted;
	}

	public override void Undo() =>
		Global.Canvas.Animation.InsertFrame(Frame, Index);

	public override void Redo() =>
		Global.Canvas.Animation.RemoveFrame(Frame.Id);
}
