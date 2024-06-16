using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionChangedHistoryAction : HistoryAction
{
	public Dictionary<Vector2I, SelectionChange> SelectionChanges { get; } = new();

	public SelectionChangedHistoryAction()
	{
		HasChanges = false;
		ActionType = HistoryActionType.SelectionChanged;
	}

	public void AddSelectionChange(SelectionChange change)
	{
		if (SelectionChanges.TryGetValue(change.Position, out SelectionChange oldChange))
		{
			SelectionChanges[change.Position] = new(change.Position,
				oldChange.OldSelected, change.NewSelected);
		}
		else
			SelectionChanges.Add(change.Position, change);
		HasChanges = true;
	}

	public override void Undo()
	{
		foreach (var change in SelectionChanges.Values)
			Global.Canvas.Selection.SetPixel(change.Position, change.OldSelected);
		Global.Canvas.Selection.Update();
	}

	public override void Redo()
	{
		foreach (var change in SelectionChanges.Values)
			Global.Canvas.Selection.SetPixel(change.Position, change.NewSelected);
		Global.Canvas.Selection.Update();
	}
}
