using System;
using Godot;
using Scribble.Application;

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

	public override void _Ready()
	{
		Global.AnimationTimeline = this;

		SetupButtons();

		framePanelContainer.GuiInput += FramePanelContainerRightClicked;

		//Dont hide if the current animation has more than 1 frame
		Hide();
	}

	private void SetupButtons()
	{
		closeButton.Pressed += Hide;
		settingsButton.Pressed += () => WindowManager.Get("animation").Show();
		//addFrameButton.Pressed += AddFrame;
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

	public void AddFrame()
	{
		//...
	}

	internal void Update()
	{
		//...
	}
}
