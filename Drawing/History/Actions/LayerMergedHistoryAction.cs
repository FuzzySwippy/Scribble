using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class LayerMergedHistoryAction : HistoryAction
{
	private Layer MergedLayer { get; }
	private int MergedLayerIndex { get; }
	private Layer TargetLayer { get; }
	private Color[,] TargetLayerColors { get; }

	public LayerMergedHistoryAction(Layer mergedLayer, int mergedLayerIndex, Layer targetLayer)
	{
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
		Global.Canvas.RestoreLayer(MergedLayer, MergedLayerIndex);
	}

	public override void Redo() =>
		Global.Canvas.MergeDown(MergedLayerIndex, false);
}
