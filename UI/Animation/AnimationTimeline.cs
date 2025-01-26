using System;
using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI.Animation;

namespace Scribble.UI;

public partial class AnimationTimeline : Control
{
	private Drawing.Animation Animation => Global.Canvas.Animation;

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
	private List<Control> FrameControls { get; } = [];
	private List<AnimationFrame> AnimationFrames { get; } = [];

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

	private DateTime LastFramePlayTime { get; set; }

	public override void _Ready()
	{
		Global.AnimationTimeline = this;

		BackgroundTexture = TextureGenerator.NewBackgroundTexture((Canvas.ChunkSize / 4).ToVector2I());

		SetupButtons();

		Hide();
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

	#region Controls
	private void Play() => Playing = !Playing;

	internal void AddFrame()
	{
		Animation.NewFrame();
		Update();
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

		foreach (AnimationFrame animationFrame in AnimationFrames)
			animationFrame.Deselect();
		AnimationFrames[index].Select();

		LastFramePlayTime = DateTime.Now;
	}
	#endregion

	#region Drag
	public void FrameStartedDragging(AnimationFrame frame)
	{
		FrameControls.Remove(frame);
		AnimationFrames.Remove(frame);

		DraggedFrame = frame;
	}

	public void FrameEndedDragging() =>
		DraggedFrame = null;
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

			FrameControls.Add(animationFrame);
			AnimationFrames.Add(animationFrame);

			if (frame == Animation.CurrentFrame)
				animationFrame.Select();
		}

		AddFrameInsertPosition(Animation.Frames.Count);
	}

	private void AddFrameInsertPosition(int index)
	{
		AnimationFrameInsertPosition insertPosition = insertPositionPrefab.Instantiate<AnimationFrameInsertPosition>();
		insertPosition.Init(index);
		frameContainer.AddChild(insertPosition);

		FrameControls.Add(insertPosition);
	}

	private void ClearFrameControls()
	{
		foreach (AnimationFrame animationFrame in AnimationFrames)
			animationFrame.UnInit();

		foreach (Control control in FrameControls)
			control.QueueFree();

		FrameControls.Clear();
		AnimationFrames.Clear();
	}
	#endregion
}
