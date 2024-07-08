using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class PasteHistoryAction : HistoryAction
{
	private ulong CreatedLayerId { get; }
	private Vector2I MousePos { get; }
	private Image Image { get; }

	public PasteHistoryAction(ulong createdLayerId, Vector2I mousePos, Image image)
	{
		ActionType = HistoryActionType.Paste;
		CreatedLayerId = createdLayerId;
		MousePos = mousePos;
		Image = image;

		HasChanges = true;
	}

	public override void Undo() =>
		Global.Canvas.DeleteLayer(Global.Canvas.GetLayerIndex(CreatedLayerId), false);

	public override void Redo() =>
		Global.Canvas.Paste(MousePos, Image);
}
