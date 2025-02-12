using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class LayerMergedHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private Layer MergedLayer { get; }
	private int MergedLayerIndex { get; }
	private Layer TargetLayer { get; }
	private Color[,] TargetLayerColors { get; }

	public LayerMergedHistoryAction(ulong frameId, Layer mergedLayer, int mergedLayerIndex, Layer targetLayer)
	{
		FrameId = frameId;
		MergedLayer = mergedLayer;
		MergedLayerIndex = mergedLayerIndex;
		TargetLayer = targetLayer;
		TargetLayerColors = (Color[,])TargetLayer.Colors.Clone();

		HasChanges = true;
		ActionType = HistoryActionType.LayerMerged;
	}

	public override void Undo()
	{
		TargetLayer.Colors = TargetLayerColors;
		Global.Canvas.SelectFrameAndRestoreLayer(FrameId, MergedLayer, MergedLayerIndex);
	}

	public override void Redo() =>
		Global.Canvas.SelectFrameAndMergeLayer(FrameId, MergedLayerIndex, false);
}
