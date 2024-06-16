using Scribble.Application;

namespace Scribble.Drawing;

public class LayerMovedHistoryAction : HistoryAction
{
	private int FromIndex { get; }
	private int ToIndex { get; }

	public LayerMovedHistoryAction(int fromIndex, int toIndex)
	{
		FromIndex = fromIndex;
		ToIndex = toIndex;
		HasChanges = true;
		ActionType = HistoryActionType.LayerMoved;
	}

	public override void Undo() =>
		Global.Canvas.SetLayerIndex(ToIndex, FromIndex);

	public override void Redo() =>
		Global.Canvas.SetLayerIndex(FromIndex, ToIndex);
}
