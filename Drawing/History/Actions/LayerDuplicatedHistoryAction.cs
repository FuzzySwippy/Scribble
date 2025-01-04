using Scribble.Application;

namespace Scribble.Drawing;

public class LayerDuplicatedHistoryAction : HistoryAction
{
	public ulong FrameId { get; }
	private int DuplicateIndex { get; }

	public LayerDuplicatedHistoryAction(ulong frameId, int duplicateIndex)
	{
		FrameId = frameId;
		DuplicateIndex = duplicateIndex;

		HasChanges = true;
		ActionType = HistoryActionType.LayerCreated;
	}

	public override void Undo() =>
		Global.Canvas.SelectFrameAndDeleteLayer(FrameId, DuplicateIndex, false);

	public override void Redo() =>
		Global.Canvas.SelectFrameAndDuplicateLayer(FrameId, DuplicateIndex, false);
}
