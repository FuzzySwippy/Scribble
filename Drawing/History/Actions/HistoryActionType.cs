namespace Scribble.Drawing;

public enum HistoryActionType
{
	//Drawing
	DrawPencil,
	DrawDither,
	DrawRectangle,
	DrawLine,
	DrawFlood,
	DrawGradient,

	//Selecting
	SelectionChanged,
	SelectionOffsetChanged,
	SelectionCleared,
	SelectionMoved,
	SelectionRotated,
	SelectionScaled,

	//Layers
	LayerDeleted,
	LayerCreated,
	LayerMoved,
	LayerMerged,
	LayerDuplicated,
	LayerOpacityChanged,
	LayerNameChanged,
	LayerVisibilityChanged,
	LayerBlendModeChanged,

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
	FrameDuplicated,

	//Tools
	ReplaceColor
}
