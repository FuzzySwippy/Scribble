using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class ReplaceColorHistoryAction : HistoryAction
{
	//Key 1: FrameId  Key 2: LayerId  Key 3: Position
	private Dictionary<ulong, Dictionary<ulong, Dictionary<Vector2I, PixelChange>>> PixelChanges { get; } = new();

	public ReplaceColorHistoryAction()
	{
		HasChanges = true;
		ActionType = HistoryActionType.ReplaceColor;
	}

	public void AddPixelChange(PixelChange pixelChange, ulong frameId, ulong layerId)
	{
		if (!PixelChanges.TryGetValue(frameId, out Dictionary<ulong, Dictionary<Vector2I, PixelChange>> framePixelChanges))
		{
			framePixelChanges = new();
			PixelChanges.Add(frameId, framePixelChanges);
		}

		if (!framePixelChanges.TryGetValue(layerId, out Dictionary<Vector2I, PixelChange> layerPixelChanges))
		{
			layerPixelChanges = new();
			framePixelChanges.Add(layerId, layerPixelChanges);
		}
		
		if (layerPixelChanges.TryGetValue(pixelChange.Position, out PixelChange oldPixelChange))
			layerPixelChanges[pixelChange.Position] = new(pixelChange.Position, oldPixelChange.OldColor, pixelChange.NewColor);
		else
			layerPixelChanges.Add(pixelChange.Position, pixelChange);
		HasChanges = true;
	}

	public override void Undo()
	{
		foreach (ulong frameId in PixelChanges.Keys)
		{
			Frame frame = Global.Canvas.Animation.GetFrame(frameId);
			foreach (ulong layerId in PixelChanges[frameId].Keys)
			{
				Layer layer = frame.GetLayer(layerId);
				foreach (PixelChange pixelChange in PixelChanges[frameId][layerId].Values)
					Global.Canvas.SetPixelInLayer(pixelChange.Position, pixelChange.OldColor, layer);
				layer.PreviewNeedsUpdate = true;
			}
			frame.PreviewNeedsUpdate = true;
		}
	}

	public override void Redo()
	{
		foreach (ulong frameId in PixelChanges.Keys)
		{
			Frame frame = Global.Canvas.Animation.GetFrame(frameId);
			foreach (ulong layerId in PixelChanges[frameId].Keys)
			{
				Layer layer = frame.GetLayer(layerId);
				foreach (PixelChange pixelChange in PixelChanges[frameId][layerId].Values)
					Global.Canvas.SetPixelInLayer(pixelChange.Position, pixelChange.NewColor, layer);
				layer.PreviewNeedsUpdate = true;
			}
			frame.PreviewNeedsUpdate = true;
		}
	}
}
