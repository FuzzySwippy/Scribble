using System.Collections.Generic;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class CutHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private ulong LayerId { get; }
	private Dictionary<Vector2I, Color> PixelChanges { get; } = [];

	//Build data
	private (Vector2I, Color)[] PixelChangesArray { get; set; }

	public CutHistoryAction(ulong frameId, ulong layerId)
	{
		ActionType = HistoryActionType.Cut;
		LayerId = layerId;
		FrameId = frameId;
	}

	public void AddPixelChange(Vector2I position, Color oldColor)
	{
		PixelChanges.Add(position, oldColor);
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (var pixelChange in PixelChangesArray)
			Global.Canvas.SetPixel(pixelChange.Item1, pixelChange.Item2);
	}

	public override void Redo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (var pixelChange in PixelChangesArray)
			Global.Canvas.SetPixel(pixelChange.Item1, new());
	}

	public override void Build()
	{
		PixelChangesArray = new (Vector2I, Color)[PixelChanges.Count];
		int i = 0;
		foreach (Vector2I pos in PixelChanges.Keys)
		{
			PixelChangesArray[i] = (pos, PixelChanges[pos]);
			i++;
		}

		PixelChanges.Clear();
	}
}
