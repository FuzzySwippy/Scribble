using System;
using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.UI.Animation;

namespace Scribble.UI;

public partial class AnimationTimeline : Control
{
	private Drawing.Animation Animation => Global.Canvas.Animation;

	[ExportGroup("Shortcuts")]
	[Export] private Shortcut duplicateFrameShortcut;
	[Export] private Shortcut deleteFrameShortcut;

	[ExportGroup("Play Button")]
	[Export] private Texture2D playTexture;
	[Export] private Texture2D pauseTexture;

	[ExportGroup("Buttons")]
	[Export] private Button closeButton;
	[Export] private Button settingsButton;
	[Export] private Button addFrameButton;
	[Export] private Button rewindButton;
	[Export] private Button frameBackButton;
	[Export] private Button playButton;
	[Export] private Button frameForwardButton;
	[Export] private Button fastForwardButton;

	[ExportGroup("Prefabs")]
	[Export] private PackedScene framePrefab;
	[Export] private PackedScene insertPositionPrefab;

	[ExportGroup("Controls")]
	[Export] private PanelContainer framePanelContainer;
	[Export] private HBoxContainer frameContainer;

	[ExportGroup("Scroll")]
	[Export] private ScrollContainer scrollContainer;
	[Export] private float dragScrollSpeed = 200f;
	[Export] private float fastDragScrollSpeed = 500f;

	[ExportSubgroup("Drag Scroll Areas")]
	[Export] private Control fastLeftDragScrollArea;
	[Export] private Control slowLeftDragScrollArea;
	[Export] private Control fastRightDragScrollArea;
	[Export] private Control slowRightDragScrollArea;


	private Texture2D BackgroundTexture { get; set; }
	private List<AnimationFrame> AnimationFrames { get; } = [];
	private List<AnimationFrameInsertPosition> InsertPositions { get; } = [];
	internal int AnimationFrameCount => Animation.Frames.Count;

	//Drag
	public AnimationFrame DraggedFrame { get; set; }

	//Playing
	private bool playing;
	public bool Playing
	{
		get => playing;
		set
		{
			playing = value;
			playButton.Icon = playing ? pauseTexture : playTexture;
			if (playing)
				LastFramePlayTime = DateTime.Now;
		}
	}

	//Shortcuts
	private KeyCombination DuplicateFrameKeyCombination { get; set; }
	private KeyCombination DeleteFrameKeyCombination { get; set; }

	private DateTime LastFramePlayTime { get; set; }

	//History
	private int HistoryMovedFromIndex { get; set; }

	public override void _Ready()
	{
		Global.AnimationTimeline = this;

		BackgroundTexture = TextureGenerator.NewBackgroundTexture((Canvas.ChunkSize / 4).ToVector2I());

		SetupButtons();

		Hide();

		//Shortcuts
		DuplicateFrameKeyCombination = new((InputEventKey)duplicateFrameShortcut.Events[0]);
		DeleteFrameKeyCombination = new((InputEventKey)deleteFrameShortcut.Events[0]);

		//Events
		Keyboard.KeyDown += HandleKeyDown;
	}

	public override void _Process(double delta)
	{
		//Playing
		if (Playing)
		{
			if (Animation.Frames.Count <= 1)
				Playing = false;
			else if ((DateTime.Now - LastFramePlayTime).TotalMilliseconds >= Animation.FrameTimeMs)
			{
				if (Animation.CurrentFrameIndex == Animation.Frames.Count - 1 && !Animation.Loop)
					Playing = false;
				else
					Animation.SelectFrameByIndex(Animation.CurrentFrameIndex + 1);
			}
		}

		//Dragging
		if (DraggedFrame == null)
			return;

		Vector2 mousePosition = GetGlobalMousePosition();

		if (fastLeftDragScrollArea.GetGlobalRect().HasPoint(mousePosition))
			scrollContainer.GetHScrollBar().Value -= fastDragScrollSpeed * delta;
		else if (slowLeftDragScrollArea.GetGlobalRect().HasPoint(mousePosition))
			scrollContainer.GetHScrollBar().Value -= dragScrollSpeed * delta;
		else if (fastRightDragScrollArea.GetGlobalRect().HasPoint(mousePosition))
			scrollContainer.GetHScrollBar().Value += fastDragScrollSpeed * delta;
		else if (slowRightDragScrollArea.GetGlobalRect().HasPoint(mousePosition))
			scrollContainer.GetHScrollBar().Value += dragScrollSpeed * delta;
	}

	private void SetupButtons()
	{
		closeButton.Pressed += Hide;
		settingsButton.Pressed += () => WindowManager.Get("animation").Show();
		addFrameButton.Pressed += AddFrame;
		rewindButton.Pressed += Rewind;
		frameBackButton.Pressed += FrameBack;
		playButton.Pressed += Play;
		frameForwardButton.Pressed += FrameForward;
		fastForwardButton.Pressed += FastForward;
	}

	private void UpdateButtons()
	{
		rewindButton.Disabled = Animation.Frames.Count <= 1;
		frameBackButton.Disabled = Animation.Frames.Count <= 1;
		playButton.Disabled = Animation.Frames.Count <= 1;
		frameForwardButton.Disabled = Animation.Frames.Count <= 1;
		fastForwardButton.Disabled = Animation.Frames.Count <= 1;
	}

	public void Toggle()
	{
		if (Visible)
			Hide();
		else
			Show();
	}

	private void HandleKeyDown(KeyCombination keyCombination)
	{
		if (keyCombination == DuplicateFrameKeyCombination)
			DuplicateFrame();
		else if (keyCombination == DeleteFrameKeyCombination)
			DeleteFrame();
	}

	#region Controls
	private void Play() => Playing = !Playing;

	internal void AddFrame()
	{
		ulong newFrameId = Animation.NewFrame(BackgroundType.Transparent, true);
		Update();
		SelectFrame(newFrameId);
	}

	private void DuplicateFrame()
	{
		foreach (AnimationFrame frame in AnimationFrames)
		{
			if (frame.IsSelected)
			{
				frame.Duplicate();
				return;
			}
		}
	}

	private void DeleteFrame()
	{
		if (Animation.Frames.Count <= 1)
			return;

		foreach (AnimationFrame frame in AnimationFrames)
		{
			if (frame.IsSelected)
			{
				frame.Delete();
				return;
			}
		}
	}

	private void Rewind()
	{
		if (Animation.CurrentFrameIndex == 0)
			return;

		Animation.SelectFrameByIndex(0);
	}

	private void FastForward()
	{
		if (Animation.CurrentFrameIndex == Animation.Frames.Count - 1)
			return;

		Animation.SelectFrameByIndex(Animation.Frames.Count - 1);
	}

	private void FrameBack()
	{
		int newIndex = Animation.CurrentFrameIndex - 1;
		if (newIndex < 0)
			newIndex = Animation.Frames.Count - 1;

		Animation.SelectFrameByIndex(newIndex);
	}

	private void FrameForward()
	{
		int newIndex = Animation.CurrentFrameIndex + 1;
		if (newIndex >= Animation.Frames.Count)
			newIndex = 0;

		Animation.SelectFrameByIndex(newIndex);
	}

	public void SelectFrame(ulong frameId)
	{
		int index = Animation.GetFrameIndex(frameId);
		if (index == -1)
			return;

		Animation.CurrentFrameIndex = index;

		foreach (AnimationFrame animationFrame in AnimationFrames)
			animationFrame.Deselect();
		AnimationFrames[index].Select();

		LastFramePlayTime = DateTime.Now;
	}
	#endregion

	#region Drag
	public void FrameStartedDragging(AnimationFrame frame)
	{
		AnimationFrames.Remove(frame);

		DraggedFrame = frame;

		//Record history
		HistoryMovedFromIndex = Animation.GetFrameIndex(frame.Frame.Id);
	}

	public void FrameEndedDragging(int newIndex)
	{
		//Record history
		if (HistoryMovedFromIndex != newIndex)
		{
			GD.Print($"FrameMovedHistoryAction: {DraggedFrame.Frame.Id} from {HistoryMovedFromIndex} to {newIndex}");
			Global.Canvas.History.AddAction(new FrameMovedHistoryAction(DraggedFrame.Frame.Id, HistoryMovedFromIndex, newIndex));
		}

		DraggedFrame = null;
	}
	#endregion

	#region Update
	internal void Update()
	{
		ClearFrameControls();

		for (int i = 0; i < Animation.Frames.Count; i++)
		{
			AddFrameInsertPosition(i);

			Frame frame = Animation.Frames[i];
			AnimationFrame animationFrame = framePrefab.Instantiate<AnimationFrame>();

			animationFrame.Init(frame, i, BackgroundTexture, frame.Preview);
			frameContainer.AddChild(animationFrame);

			AnimationFrames.Add(animationFrame);

			if (frame == Animation.CurrentFrame)
				animationFrame.Select();
		}

		AddFrameInsertPosition(Animation.Frames.Count);

		UpdateButtons();
	}

	private void AddFrameInsertPosition(int index)
	{
		AnimationFrameInsertPosition insertPosition = insertPositionPrefab.Instantiate<AnimationFrameInsertPosition>();
		insertPosition.Init(index);
		frameContainer.AddChild(insertPosition);

		InsertPositions.Add(insertPosition);
	}

	private void ClearFrameControls()
	{
		foreach (AnimationFrame animationFrame in AnimationFrames)
		{
			animationFrame.UnInit();
			animationFrame.QueueFree();
		}

		foreach (AnimationFrameInsertPosition insertPosition in InsertPositions)
			insertPosition.QueueFree();

		AnimationFrames.Clear();
		InsertPositions.Clear();
	}
	#endregion
}
