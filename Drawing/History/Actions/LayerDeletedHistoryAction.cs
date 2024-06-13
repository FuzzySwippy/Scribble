using Scribble.Application;

namespace Scribble.Drawing;

public class LayerDeletedHistoryAction : HistoryAction
{
	private Layer Layer { get; }
	private int Index { get; }

	public LayerDeletedHistoryAction(Layer layer, int index)
	{
		Layer = layer;
		Index = index;
		HasChanges = true;
		ActionType = HistoryActionType.LayerDeleted;
	}

	public override void Undo() =>
		Global.Canvas.RestoreLayer(Layer, Index);

	public override void Redo() =>
		Global.Canvas.DeleteLayer(Index, false);
}
