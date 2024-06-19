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
	SelectionChanged,
	SelectionOffsetChanged,
	SelectionCleared,
	SelectionMoved,

	//Layers
	LayerDeleted,
	LayerCreated,
	LayerMoved,
	LayerMerged,
	LayerDuplicated,
	LayerOpacityChanged,
	LayerNameChanged,
	LayerVisibilityChanged,

	//ImageOperations
	FlippedVertically,
	FlippedHorizontally,
	RotatedClockwise,
	RotatedCounterClockwise,
	ResizeCanvas
}
