using Scribble.Application;

namespace Scribble.Drawing;

public class LayerNameChangedHistoryAction : HistoryAction
{
	private int LayerIndex { get; }
	private string OldName { get; }
	private string NewName { get; set; }

	public LayerNameChangedHistoryAction(int layerIndex, string oldName, string newName)
	{
		LayerIndex = layerIndex;
		OldName = oldName;
		NewName = newName;

		TryMerge = true;
		HasChanges = true;
		ActionType = HistoryActionType.LayerNameChanged;
	}

	public override void Undo() =>
		Global.LayerEditor.SetLayerName(LayerIndex, OldName, false);

	public override void Redo() =>
		Global.LayerEditor.SetLayerName(LayerIndex, NewName, false);

	public override bool Merge(HistoryAction action)
	{
		if (!base.Merge(action))
			return false;

		LayerNameChangedHistoryAction layerNameChangedAction =
			(LayerNameChangedHistoryAction)action;

		if (LayerIndex != layerNameChangedAction.LayerIndex)
			return false;

		NewName = layerNameChangedAction.NewName;
		return true;
	}
}
