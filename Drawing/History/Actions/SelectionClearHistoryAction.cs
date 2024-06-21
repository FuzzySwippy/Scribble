using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionClearHistoryAction : HistoryAction
{
	private Vector2I Offset { get; }
	private HashSet<Vector2I> OldSelectionHashSet { get; } = new();
	private List<Vector2I> OldSelection { get; } = new();

	//Build data
	private Vector2I[] OldSelectionArray { get; set; }

	public SelectionClearHistoryAction(Vector2I offset)
	{
		Offset = offset;
		ActionType = HistoryActionType.SelectionCleared;
	}

	public void AddSelection(Vector2I position)
	{
		if (OldSelectionHashSet.Contains(position))
			return;

		OldSelection.Add(position);
		OldSelectionHashSet.Add(position);
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.Selection.Offset = Offset;
		foreach (var selection in OldSelectionArray)
			Global.Canvas.Selection.SetPixel(selection, true);
		Global.Canvas.Selection.Update();
	}

	public override void Redo() =>
		Global.Canvas.Selection.Clear(false);

	public override void Build()
	{
		OldSelectionArray = OldSelection.ToArray();
		OldSelection.Clear();
		OldSelectionHashSet.Clear();
	}
}
