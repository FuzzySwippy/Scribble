namespace Scribble.Drawing;

public enum HistoryActionType
{
	//Drawing
	DrawPencil,
	DrawDither,
	DrawRectangle,
	DrawLine,
	DrawFlood,

	//Selecting
	SelectionChanged,
	SelectionOffsetChanged,
	SelectionCleared,
	SelectionMoved,
	SelectionRotated,

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
	ResizeCanvas,
	CropToContent,
	Cut,
	Paste,
	ClearPixels,

	//Frames
	FrameCreated,
	FrameDeleted,
	FrameMoved,
	FrameDuplicated
}
