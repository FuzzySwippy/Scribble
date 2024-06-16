using Scribble.Application;

namespace Scribble.Drawing;

public class RotatedCounterClockwiseHistoryAction : HistoryAction
{
	public RotatedCounterClockwiseHistoryAction()
	{
		HasChanges = true;
		ActionType = HistoryActionType.RotatedCounterClockwise;
	}

	public override void Undo() =>
		Global.Canvas.RotateClockwise(false);

	public override void Redo() =>
		Global.Canvas.RotateCounterClockwise(false);
}
