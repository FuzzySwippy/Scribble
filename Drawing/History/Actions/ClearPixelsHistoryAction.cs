using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class ClearPixelsHistoryAction : HistoryAction
{
	private ulong LayerId { get; }
	private Dictionary<Vector2I, Color> OldPixels { get; } = new();

	public ClearPixelsHistoryAction(ulong layerId)
	{
		ActionType = HistoryActionType.ClearPixels;
		LayerId = layerId;
	}

	public void AddPixelChange(Vector2I position, Color oldColor)
	{
		OldPixels.Add(position, oldColor);
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectLayer(LayerId);
		foreach (Vector2I pos in OldPixels.Keys)
			Global.Canvas.SetPixel(pos, OldPixels[pos]);
	}

	public override void Redo()
	{
		Global.Canvas.SelectLayer(LayerId);
		foreach (Vector2I pos in OldPixels.Keys)
			Global.Canvas.SetPixel(pos, new());
	}
}
