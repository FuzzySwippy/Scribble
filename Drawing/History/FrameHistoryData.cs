namespace Scribble.Drawing;

public class FrameHistoryData(ulong frameId, Frame oldFrame)
{
	public ulong FrameId { get; } = frameId;
	public Frame OldFrame { get; } = oldFrame;
}
