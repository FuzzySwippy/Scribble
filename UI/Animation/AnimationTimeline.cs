using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.Drawing;
using Scribble.ScribbleLib.Extensions;
using Scribble.UI.Animation;

namespace Scribble.UI;

public partial class AnimationTimeline : Control
{
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

	public override void _Ready()
	{
		Global.AnimationTimeline = this;

		BackgroundTexture = TextureGenerator.NewBackgroundTexture((Canvas.ChunkSize / 4).ToVector2I());

		SetupButtons();

		framePanelContainer.GuiInput += FramePanelContainerRightClicked;

		//Dont hide if the current animation has more than 1 frame
		Hide();
	}

	public override void _Process(double delta)
	{
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
		//rewindButton.Pressed += Rewind;
		//frameBackButton.Pressed += FrameBack;
		//playButton.Pressed += Play;
		//frameForwardButton.Pressed += FrameForward;
		//fastForwardButton.Pressed += FastForward;
	}

	private void FramePanelContainerRightClicked(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed)
		{
			ContextMenu.ShowMenu(mouseEvent.GlobalPosition,
			[
				new("Add Frame", AddFrame)
			]);
		}
	}

	internal void AddFrame()
	{
		Global.Canvas.Animation.NewFrame();
		Update();
	}

	public void SelectFrame(ulong frameId)
	{
		int index = Global.Canvas.Animation.GetFrameIndex(frameId);
		if (index == -1)
			return;

		foreach (AnimationFrame animationFrame in AnimationFrames)
			animationFrame.Deselect();
		AnimationFrames[index].Select();
	}

	public void FrameStartedDragging(AnimationFrame frame)
	{
		FrameControls.Remove(frame);
		AnimationFrames.Remove(frame);

		DraggedFrame = frame;
	}

	public void FrameEndedDragging() =>
		DraggedFrame = null;

	internal void Update()
	{
		ClearFrameControls();

		for (int i = 0; i < Global.Canvas.Animation.Frames.Count; i++)
		{
			AddFrameInsertPosition(i);

			Frame frame = Global.Canvas.Animation.Frames[i];
			AnimationFrame animationFrame = framePrefab.Instantiate<AnimationFrame>();

			animationFrame.Init(frame, i, BackgroundTexture, frame.Preview);
			frameContainer.AddChild(animationFrame);

			FrameControls.Add(animationFrame);
			AnimationFrames.Add(animationFrame);

			if (frame == Global.Canvas.Animation.CurrentFrame)
				animationFrame.Select();
		}

		AddFrameInsertPosition(Global.Canvas.Animation.Frames.Count);
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
}
