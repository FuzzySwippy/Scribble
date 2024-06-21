using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionChangedHistoryAction : HistoryAction
{
	private Dictionary<Vector2I, SelectionChange> SelectionChanges { get; } = new();

	//Build data
	private SelectionChange[] SelectionChangesArray { get; set; }

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
		foreach (var change in SelectionChangesArray)
			Global.Canvas.Selection.SetPixel(change.Position, change.OldSelected);
		Global.Canvas.Selection.Update();
	}

	public override void Redo()
	{
		foreach (var change in SelectionChangesArray)
			Global.Canvas.Selection.SetPixel(change.Position, change.NewSelected);
		Global.Canvas.Selection.Update();
	}

	public override void Build()
	{
		SelectionChangesArray = SelectionChanges.Values.ToArray();
		SelectionChanges.Clear();
	}
}
