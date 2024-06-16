using Scribble.Application;

namespace Scribble.Drawing;

public class LayerVisibilityChangedHistoryAction : HistoryAction
{
	private int LayerIndex { get; }
	private bool OldVisibility { get; }
	private bool NewVisibility { get; set; }

	public LayerVisibilityChangedHistoryAction(int layerIndex, bool oldVisibility, bool newVisibility)
	{
		LayerIndex = layerIndex;
		OldVisibility = oldVisibility;
		NewVisibility = newVisibility;

		HasChanges = true;
		ActionType = HistoryActionType.LayerVisibilityChanged;
	}

	public override void Undo() =>
		Global.LayerEditor.SetLayerVisibility(LayerIndex, OldVisibility, false);

	public override void Redo() =>
		Global.LayerEditor.SetLayerVisibility(LayerIndex, NewVisibility, false);
}
