using Scribble.Application;

namespace Scribble.Drawing;

public class RotatedClockwiseHistoryAction : HistoryAction
{
	public RotatedClockwiseHistoryAction()
	{
		HasChanges = true;
		ActionType = HistoryActionType.RotatedClockwise;
	}

	public override void Undo() =>
		Global.Canvas.RotateCounterClockwise(false);

	public override void Redo() =>
		Global.Canvas.RotateClockwise(false);
}
