namespace Scribble.Drawing;

public enum HistoryActionType
{
	//Drawing
	DrawPencilRound,
	DrawPencilSquare,
	DrawRectangle,
	DrawLine,
	DrawFlood,

	//Selecting

	//Layers
	LayerDeleted,
	LayerCreated,
	LayerMoved,
	LayerMerged,
	LayerDuplicated,
	LayerOpacityChanged,

	LayerNameChanged,
	LayerVisibilityChanged,
}
