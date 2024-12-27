using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class SelectionRotatedHistoryAction : HistoryAction
{
	private ulong LayerId { get; }

	private Dictionary<Vector2I, Color> SelectionPixels { get; } = new();
	private List<Vector2I> OldSelectionPixels { get; } = new();
	private List<Vector2I> NewSelectionPixels { get; } = new();
	private Dictionary<Vector2I, PixelChange> OverwrittenPixels { get; } = new();

	public SelectionRotatedHistoryAction(ulong layerId)
	{
		LayerId = layerId;

		ActionType = HistoryActionType.SelectionRotated;
	}

	public void AddSelectionPixel(Vector2I position, Color color) =>
		SelectionPixels[position] = color;

	public void AddOldSelectionPixel(Vector2I position) =>
		OldSelectionPixels.Add(position);

	public void AddNewSelectionPixel(Vector2I position) =>
		NewSelectionPixels.Add(position);

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

		foreach (var pos in SelectionPixels.Keys)
			Global.Canvas.SetPixel(pos, SelectionPixels[pos]);

		Global.Canvas.Selection.Clear(false);
		foreach (var pos in OldSelectionPixels)
			Global.Canvas.Selection.SetPixel(pos);
		Global.Canvas.Selection.Update();
	}

	public override void Redo()
	{
		Global.Canvas.SelectLayer(LayerId);

		foreach (var pos in SelectionPixels.Keys)
			Global.Canvas.SetPixel(pos, new());

		foreach (var pos in OverwrittenPixels.Keys)
			Global.Canvas.SetPixel(pos, OverwrittenPixels[pos].NewColor);

		Global.Canvas.Selection.Clear(false);
		foreach (var pos in NewSelectionPixels)
			Global.Canvas.Selection.SetPixel(pos);
		Global.Canvas.Selection.Update();
	}
}