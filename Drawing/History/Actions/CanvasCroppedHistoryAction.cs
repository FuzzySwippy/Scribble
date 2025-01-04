using Godot;
using Scribble.Application;

namespace Scribble.Drawing;

public class CanvasCroppedHistoryAction : HistoryAction
{
	private Vector2I OldSize { get; }
	private CropType CropType { get; }
	private FrameHistoryData[] FrameHistoryData { get; }

	public CanvasCroppedHistoryAction(Vector2I oldSize, CropType type,
		FrameHistoryData[] frameHistoryData)
	{
		OldSize = oldSize;
		CropType = type;
		FrameHistoryData = frameHistoryData;

		ActionType = HistoryActionType.CropToContent;
		HasChanges = true;
	}

	public override void Undo() =>
		Global.Canvas.ResizeWithFrameData(OldSize, FrameHistoryData);

	public override void Redo() =>
		Global.Canvas.CropToContent(CropType, false);
}
