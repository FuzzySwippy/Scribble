using Scribble.Application;

namespace Scribble.Drawing;

public class LayerOpacityChangedHistoryAction : HistoryAction
{
	private int LayerIndex { get; }
	private float OldOpacity { get; }
	private float NewOpacity { get; set; }

	public LayerOpacityChangedHistoryAction(int layerIndex, float oldOpacity, float newOpacity)
	{
		LayerIndex = layerIndex;
		OldOpacity = oldOpacity;
		NewOpacity = newOpacity;

		TryMerge = true;
		HasChanges = true;
		ActionType = HistoryActionType.LayerOpacityChanged;
	}

	public override void Undo() =>
		Global.LayerEditor.SetLayerOpacity(LayerIndex, OldOpacity, false);

	public override void Redo() =>
		Global.LayerEditor.SetLayerOpacity(LayerIndex, NewOpacity, false);

	public override bool Merge(HistoryAction action)
	{
		if (!base.Merge(action))
			return false;

		LayerOpacityChangedHistoryAction layerOpacityChangedAction =
			(LayerOpacityChangedHistoryAction)action;

		if (LayerIndex != layerOpacityChangedAction.LayerIndex)
			return false;

		NewOpacity = layerOpacityChangedAction.NewOpacity;
		return true;
	}
}
