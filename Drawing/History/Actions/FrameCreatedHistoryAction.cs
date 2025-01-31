using Scribble.Application;

namespace Scribble.Drawing;

public class FrameCreatedHistoryAction : HistoryAction
{
	private Frame Frame { get; set; }
	private int Index { get; set; }

	public FrameCreatedHistoryAction(Frame frame, int index, bool duplicate)
	{
		Frame = frame;
		Index = index;
		HasChanges = true;
		ActionType = duplicate ? HistoryActionType.FrameDuplicated : HistoryActionType.FrameCreated;
	}

	public override void Undo() =>
		Global.Canvas.Animation.RemoveFrame(Frame.Id);

	public override void Redo() =>
		Global.Canvas.Animation.InsertFrame(Frame, Index);
}
