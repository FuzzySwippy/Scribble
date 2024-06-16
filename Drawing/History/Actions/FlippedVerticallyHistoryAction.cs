using Scribble.Application;

namespace Scribble.Drawing;

public class FlippedVerticallyHistoryAction : HistoryAction
{
	public FlippedVerticallyHistoryAction()
	{
		HasChanges = true;
		ActionType = HistoryActionType.FlippedVertically;
	}

	public override void Undo() =>
		Global.Canvas.FlipVertically(false);

	public override void Redo() =>
		Global.Canvas.FlipVertically(false);
}
