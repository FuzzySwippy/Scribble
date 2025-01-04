using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class DrawHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private ulong LayerId { get; }
	private Dictionary<Vector2I, PixelChange> PixelChanges { get; } = new();

	//Build data
	private PixelChange[] PixelChangesArray { get; set; }

	public DrawHistoryAction(HistoryActionType actionType, ulong layerId, ulong frameId)
	{
		ActionType = actionType;
		LayerId = layerId;
		FrameId = frameId;
	}

	public void AddPixelChange(PixelChange pixelChange)
	{
		if (PixelChanges.TryGetValue(pixelChange.Position, out PixelChange oldPixelChange))
		{
			PixelChanges[pixelChange.Position] = new(pixelChange.Position,
				oldPixelChange.OldColor, pixelChange.NewColor);
		}
		else
			PixelChanges.Add(pixelChange.Position, pixelChange);
		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (PixelChange pixelChange in PixelChangesArray)
			Global.Canvas.SetPixel(pixelChange.Position, pixelChange.OldColor);
	}

	public override void Redo()
	{
		Global.Canvas.SelectFrameAndLayer(FrameId, LayerId);
		foreach (PixelChange pixelChange in PixelChangesArray)
			Global.Canvas.SetPixel(pixelChange.Position, pixelChange.NewColor);
	}

	public override void Build()
	{
		PixelChangesArray = PixelChanges.Values.ToArray();
		PixelChanges.Clear();
	}
}
