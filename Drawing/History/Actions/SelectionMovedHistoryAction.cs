using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionMovedHistoryAction : HistoryAction
{
	private ulong LayerId { get; }
	private Dictionary<Vector2I, Color> SelectionPixels { get; } = new();
	private Dictionary<Vector2I, PixelChange> OverwrittenPixels { get; } = new();
	private Vector2I OldOffset { get; }

	public Vector2I NewOffset { get; set; }

	public SelectionMovedHistoryAction(ulong layerId, Vector2I oldOffset)
	{
		LayerId = layerId;
		OldOffset = oldOffset;

		ActionType = HistoryActionType.SelectionMoved;
	}

	public void AddSelectionPixel(Vector2I position, Color color) =>
		SelectionPixels[position] = color;

	public void AddOverwrittenPixel(PixelChange change)
	{
		OverwrittenPixels[change.Position] = change;
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectLayer(LayerId);

		foreach (var pos in OverwrittenPixels.Keys)
			Global.Canvas.SetPixel(pos, OverwrittenPixels[pos].OldColor);

		Global.Canvas.Selection.Offset = OldOffset;

		foreach (var pos in SelectionPixels.Keys)
			Global.Canvas.SetPixel(pos, SelectionPixels[pos]);
	}

	public override void Redo()
	{
		Global.Canvas.SelectLayer(LayerId);

		foreach (var pos in SelectionPixels.Keys)
			Global.Canvas.SetPixel(pos, new());

		Global.Canvas.Selection.Offset = NewOffset;

		foreach (var pos in OverwrittenPixels.Keys)
			Global.Canvas.SetPixel(pos, OverwrittenPixels[pos].NewColor);
	}
}
