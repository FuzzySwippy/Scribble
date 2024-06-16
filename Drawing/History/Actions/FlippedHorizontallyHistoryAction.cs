using Scribble.Application;

namespace Scribble.Drawing;

public class FlippedHorizontallyHistoryAction : HistoryAction
{
	public FlippedHorizontallyHistoryAction()
	{
		HasChanges = true;
		ActionType = HistoryActionType.FlippedHorizontally;
	}

	public override void Undo() =>
		Global.Canvas.FlipHorizontally(false);

	public override void Redo() =>
		Global.Canvas.FlipHorizontally(false);
}
