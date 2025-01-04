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

	private Texture2D BackgroundTexture { get; set; }
	private List<Control> FrameControls { get; } = [];
	private List<AnimationFrame> AnimationFrames { get; } = [];

	public override void _Ready()
	{
		Global.AnimationTimeline = this;

		BackgroundTexture = TextureGenerator.NewBackgroundTexture((Canvas.ChunkSize / 4).ToVector2I());

		SetupButtons();

		framePanelContainer.GuiInput += FramePanelContainerRightClicked;

		//Dont hide if the current animation has more than 1 frame
		Hide();
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

	internal void Update()
	{
		ClearFrameControls();

		foreach (Frame frame in Global.Canvas.Animation.Frames)
		{
			AnimationFrame animationFrame = framePrefab.Instantiate<AnimationFrame>();
			animationFrame.Init(frame.Id, BackgroundTexture, frame.Preview);
			frameContainer.AddChild(animationFrame);

			FrameControls.Add(animationFrame);
			AnimationFrames.Add(animationFrame);

			if (frame == Global.Canvas.Animation.CurrentFrame)
				animationFrame.Select();
		}
	}

	private void ClearFrameControls()
	{
		foreach (Control control in FrameControls)
			control.QueueFree();
		FrameControls.Clear();
		AnimationFrames.Clear();
	}
}
