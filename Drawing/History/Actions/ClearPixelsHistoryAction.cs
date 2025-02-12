using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class ClearPixelsHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private ulong LayerId { get; }
	private Dictionary<Vector2I, Color> OldPixels { get; } = [];

	public ClearPixelsHistoryAction(ulong frameId, ulong layerId)
	{
		ActionType = HistoryActionType.ClearPixels;
		LayerId = layerId;
		FrameId = frameId;
	}

	public void AddPixelChange(Vector2I position, Color oldColor)
	{
		OldPixels.Add(position, oldColor);
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (Vector2I pos in OldPixels.Keys)
			Global.Canvas.SetPixel(pos, OldPixels[pos]);
	}

	public override void Redo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (Vector2I pos in OldPixels.Keys)
			Global.Canvas.SetPixel(pos, new());
	}
}
