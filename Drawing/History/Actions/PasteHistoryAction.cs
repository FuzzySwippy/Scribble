using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class PasteHistoryAction : HistoryAction
{
	private ulong FrameId { get; }
	private int CreatedLayerIndex { get; }
	private Vector2I MousePos { get; }
	private Image Image { get; }

	public PasteHistoryAction(ulong frameId, int createdLayerIndex, Vector2I mousePos, Image image)
	{
		ActionType = HistoryActionType.Paste;
		FrameId = frameId;
		CreatedLayerIndex = createdLayerIndex;
		MousePos = mousePos;
		Image = image;

		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.SelectFrameAndDeleteLayer(FrameId, CreatedLayerIndex, false);
		Global.Canvas.Selection.Clear(false);
	}

	public override void Redo() =>
		Global.Canvas.Paste(MousePos, Image);
}
