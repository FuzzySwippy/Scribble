using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI;

namespace Scribble.Drawing;

public class Animation
{
	private Canvas Canvas { get; }

	public List<Frame> Frames { get; } = new();

	private int currentFrameIndex;
	public int CurrentFrameIndex
	{
		get => currentFrameIndex;
		set
		{
			currentFrameIndex = value;
			Canvas.UpdateEntireCanvas();
		}
	}

	public Frame CurrentFrame => Frames.Count > currentFrameIndex ? Frames[CurrentFrameIndex] : null;

	public Animation(Canvas canvas)
	{
		Canvas = canvas;
	}

	#region Frames
	public void SelectFrame(ulong frameId)
	{
		int index = GetFrameIndex(frameId);
		if (index == -1)
			return;

		CurrentFrameIndex = index;
		Global.AnimationTimeline.Update();
	}

	public Frame GetFrame(ulong id) =>
		Frames.Find(f => f.Id == id);

	public int GetFrameIndex(ulong id) =>
		Frames.FindIndex(f => f.Id == id);

	public void SetFrames(Frame[] frames, BackgroundType backgroundType)
	{
		CurrentFrameIndex = 0;
		if (frames != null && frames.Length > 0)
			Frames.AddRange(frames);
		else
			NewFrame(backgroundType);
	}

	public void NewFrame(BackgroundType backgroundType = BackgroundType.Transparent) =>
		Frames.Add(new(Canvas, Canvas.Size, backgroundType));

	private Color[,] FlattenFrames()
	{
		Color[,] colors = new Color[Canvas.Size.X, Canvas.Size.Y];
		foreach (Frame frame in Frames)
		{
			Color[,] flattenedFrame = frame.FlattenImage();
			for (int x = 0; x < Canvas.Size.X; x++)
				for (int y = 0; y < Canvas.Size.Y; y++)
					colors[x, y] = colors[x, y].Blend(flattenedFrame[x, y]);
		}

		return colors;
	}
	#endregion

	#region ImageOperations
	public void FlipVertically(bool recordHistory = true)
	{
		Canvas.Selection.Clear();

		foreach (Frame frame in Frames)
			frame.FlipVertically();
		Canvas.UpdateEntireCanvas();
		Canvas.HasUnsavedChanges = true;

		if (recordHistory)
			Canvas.History.AddAction(new FlippedVerticallyHistoryAction());
	}

	public void FlipHorizontally(bool recordHistory = true)
	{
		Canvas.Selection.Clear();

		foreach (Frame frame in Frames)
			frame.FlipHorizontally();
		Canvas.UpdateEntireCanvas();
		Canvas.HasUnsavedChanges = true;

		if (recordHistory)
			Canvas.History.AddAction(new FlippedHorizontallyHistoryAction());
	}

	public void RotateClockwise(bool recordHistory = true)
	{
		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.Selection.Clear();

			foreach (Frame frame in Frames)
				frame.RotateClockwise();

			Canvas.Size = new(Canvas.Size.Y, Canvas.Size.X);
			Canvas.Recreate(true);

			if (recordHistory)
				Canvas.History.AddAction(new RotatedClockwiseHistoryAction());
		}
	}

	public void RotateCounterClockwise(bool recordHistory = true)
	{
		lock (Canvas.ChunkUpdateThreadLock)
		{
			Canvas.Selection.Clear();

			foreach (Frame frame in Frames)
				frame.RotateCounterClockwise();

			Canvas.Size = new(Canvas.Size.Y, Canvas.Size.X);
			Canvas.Recreate(true);

			if (recordHistory)
				Canvas.History.AddAction(new RotatedCounterClockwiseHistoryAction());
		}
	}

	public void Resize(Vector2I newSize, ResizeType type, bool recordHistory = true)
	{
		if (newSize.X == Canvas.Size.X && newSize.Y == Canvas.Size.Y)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			if (recordHistory)
				Canvas.Selection.Clear();

			Vector2I oldSize = Canvas.Size;
			Canvas.Size = newSize;

			//Resize layers
			List<FrameHistoryData> frameHistoryData = [];
			foreach (Frame frame in Frames)
				frameHistoryData.Add(new(frame.Id, frame.Resize(newSize, type)));

			Canvas.Recreate(true);

			if (recordHistory)
				Canvas.History.AddAction(
					new CanvasResizedHistoryAction(oldSize, newSize, type, frameHistoryData.ToArray()));
		}
	}

	public void ResizeWithFrameData(Vector2I newSize, FrameHistoryData[] frameHistoryData)
	{
		if (newSize.X == Canvas.Size.X && newSize.Y == Canvas.Size.Y)
			return;

		lock (Canvas.ChunkUpdateThreadLock)
		{
			Vector2I oldSize = Canvas.Size;
			Canvas.Size = newSize;

			//Resize layers
			for (int i = 0; i < Frames.Count; i++)
			{
				FrameHistoryData historyData = frameHistoryData.AsReadOnly()
					.First(f => f.FrameId == Frames[i].Id);

				Frames[i] = new(historyData.OldFrame);
			}

			Canvas.Recreate(false);
		}
	}

	public void CropToContent(CropType type, bool recordHistory = true)
	{
		FlattenFrames().CropToContent(type, out Rect2I bounds);

		if (bounds.Size == Canvas.Size || bounds.Size.X == 0 || bounds.Size.Y == 0)
		{
			WindowManager.ShowModal("Canvas is already cropped to content", ModalOptions.Ok);
			return;
		}

		if (recordHistory)
			Canvas.Selection.Clear();

		Vector2I oldSize = Canvas.Size;
		Canvas.Size = bounds.Size;

		//Resize layers
		List<FrameHistoryData> frameHistoryData = [];
		foreach (Frame frame in Frames)
			frameHistoryData.Add(new(frame.Id, frame.CropToBounds(bounds)));

		Canvas.Recreate(true);

		if (recordHistory)
			Canvas.History.AddAction(
				new CanvasCroppedHistoryAction(oldSize, type, frameHistoryData.ToArray()));
	}
	#endregion
}
