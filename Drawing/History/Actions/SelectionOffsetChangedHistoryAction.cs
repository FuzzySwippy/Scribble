using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionOffsetChangedHistoryAction : HistoryAction
{
	private Vector2I OldOffset { get; }
	private Vector2I NewOffset { get; set; }

	public SelectionOffsetChangedHistoryAction(Vector2I oldOffset, Vector2I newOffset)
	{
		OldOffset = oldOffset;
		NewOffset = newOffset;

		TryMerge = true;
		HasChanges = true;
		ActionType = HistoryActionType.SelectionOffsetChanged;
	}

	public override void Undo() =>
		Global.Canvas.Selection.Offset = OldOffset;

	public override void Redo() =>
		Global.Canvas.Selection.Offset = NewOffset;

	public override bool Merge(HistoryAction action)
	{
		if (!base.Merge(action))
			return false;

		SelectionOffsetChangedHistoryAction offsetChangedAction =
			(SelectionOffsetChangedHistoryAction)action;

		NewOffset = offsetChangedAction.NewOffset;
		return true;
	}
}
