using Scribble.Application;

namespace Scribble.Drawing;

public class LayerBlendModeChangedHistoryAction : HistoryAction
{
	private int LayerIndex { get; }
	private BlendMode OldBlendMode { get; }
	private BlendMode NewBlendMode { get; set; }

	public LayerBlendModeChangedHistoryAction(int layerIndex, BlendMode oldBlendMode, BlendMode newBlendMode)
	{
		LayerIndex = layerIndex;
		OldBlendMode = oldBlendMode;
		NewBlendMode = newBlendMode;

		TryMerge = true;
		HasChanges = true;
		ActionType = HistoryActionType.LayerBlendModeChanged;
	}

	public override void Undo() =>
		Global.LayerEditor.SetLayerBlendMode(LayerIndex, OldBlendMode, false);

	public override void Redo() =>
		Global.LayerEditor.SetLayerBlendMode(LayerIndex, NewBlendMode, false);

	public override bool Merge(HistoryAction action)
	{
		if (!base.Merge(action))
			return false;

		LayerBlendModeChangedHistoryAction layerBlendModeChangedAction =
			(LayerBlendModeChangedHistoryAction)action;

		if (LayerIndex != layerBlendModeChangedAction.LayerIndex)
			return false;

		NewBlendMode = layerBlendModeChangedAction.NewBlendMode;
		return true;
	}
}
