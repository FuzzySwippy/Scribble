using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionClearHistoryAction : HistoryAction
{
    private Vector2I Offset { get; }
	private List<Vector2I> OldSelection { get; } = new();

	public SelectionClearHistoryAction(Vector2I offset)
	{
		Offset = offset;

		TryMerge = true;
		ActionType = HistoryActionType.SelectionCleared;
	}

	public void AddSelection(Vector2I position)
	{
		if (OldSelection.Contains(position))
			return;
		OldSelection.Add(position);
		HasChanges = true;
	}

	public override bool Merge(HistoryAction action)
	{
		if (!base.Merge(action))
			return false;

		SelectionClearHistoryAction clearAction = (SelectionClearHistoryAction)action;

		foreach (var selection in clearAction.OldSelection)
			AddSelection(selection);
		return true;
	}

	public override void Undo()
	{
		Global.Canvas.Selection.Offset = Offset;
		foreach (var selection in OldSelection)
			Global.Canvas.Selection.SetPixel(selection, true);
		Global.Canvas.Selection.Update();
	}

	public override void Redo()
	{
		Global.Canvas.Selection.Offset = Offset;
		foreach (var selection in OldSelection)
			Global.Canvas.Selection.SetPixel(selection, false);
		Global.Canvas.Selection.Update();
	}
}
