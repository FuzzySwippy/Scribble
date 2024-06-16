using Scribble.Application;

namespace Scribble.Drawing;

public class LayerCreatedHistoryAction : HistoryAction
{
	private int Index { get; }

	public LayerCreatedHistoryAction(int index)
	{
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.LayerCreated;
	}

	public override void Undo() =>
		Global.Canvas.DeleteLayer(Index, false);

	public override void Redo() =>
		Global.Canvas.NewLayer(Index, false);
}
