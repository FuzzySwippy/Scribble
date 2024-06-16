using Scribble.Application;

namespace Scribble.Drawing;

public class LayerDuplicatedHistoryAction : HistoryAction
{
	private int DuplicateIndex { get; }

	public LayerDuplicatedHistoryAction(int duplicateIndex)
	{
		DuplicateIndex = duplicateIndex;

		HasChanges = true;
		ActionType = HistoryActionType.LayerCreated;
	}

	public override void Undo() =>
		Global.Canvas.DeleteLayer(DuplicateIndex, false);

	public override void Redo() =>
		Global.Canvas.DuplicateLayer(DuplicateIndex, false);
}
