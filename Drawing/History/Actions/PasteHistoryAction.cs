using System.Linq;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class PasteHistoryAction : HistoryAction
{
	private int CreatedLayerIndex { get; }
	private Vector2I MousePos { get; }
	private Image Image { get; }

	public PasteHistoryAction(int createdLayerIndex, Vector2I mousePos, Image image)
	{
		ActionType = HistoryActionType.Paste;
		CreatedLayerIndex = createdLayerIndex;
		MousePos = mousePos;
		Image = image;

		HasChanges = true;
	}

	public override void Undo()
	{
		Global.Canvas.DeleteLayer(CreatedLayerIndex, false);
		Global.Canvas.Selection.Clear(false);
	}

	public override void Redo() =>
		Global.Canvas.Paste(MousePos, Image);
}
