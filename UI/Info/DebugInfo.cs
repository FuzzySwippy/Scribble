using System.Collections.Generic;
using Godot;
using System;
using Scribble.Application;
using Scribble.ScribbleLib.Input;

namespace Scribble.UI;
public partial class DebugInfo : VBoxContainer
{
	private LabelSettings labelSettings;
	private Dictionary<string, InfoLabel> Labels { get; } = new()
	{
		{"fps_c", new ("FPS (counted)", true)},
		{"fps_d", new ("FPS (from frame time)", true)},
		{"cam_zoom", new("Camera zoom")},
		{"cam_pos", new("Camera position")},
		{"ui_scale", new("UI Scale")}
	};

	public override void _Ready()
	{
		Global.DebugInfo = this;

		CreateLabelSettings();
		GenerateLabels();

		Keyboard.KeyDown += KeyDown;
		Visible = false;
	}

	private void KeyDown(KeyCombination combination)
	{
		//Show/Hide debug info
		if (combination.key == Key.F3)
			Visible = !Visible;
	}

	public override void _Process(double delta)
	{
		if (!Visible)
			return;

		CountFPS();
		CalculateFPS(delta);
	}

	private void GenerateLabels()
	{
		foreach (string name in Labels.Keys)
		{
			Labels[name].Label = new()
			{
				LabelSettings = labelSettings
			};
			AddChild(Labels[name].Label);
		}
	}

	private void CreateLabelSettings() => labelSettings = new()
	{
		LineSpacing = 0
	};

	public static void Set(string label, object value) => Global.DebugInfo.Labels[label].Set(value);

	private float frames;
	private int lastSecond = DateTime.Now.Second;
	private void CountFPS()
	{
		frames++;
		if (DateTime.Now.Second != lastSecond)
		{
			if (DateTime.Now.Second == lastSecond + 1)
				Labels["fps_c"].Value = $"{frames}";
			else if (DateTime.Now.Second > lastSecond + 1)
				Labels["fps_c"].Value = $"{1 / (DateTime.Now.Second - lastSecond)}";

			frames = 0;
			lastSecond = DateTime.Now.Second;
		}
	}

	private DateTime fpsNextUpdateTime = DateTime.Now;
	private void CalculateFPS(double deltaTime)
	{
		if (DateTime.Now > fpsNextUpdateTime)
		{
			Labels["fps_d"].Value = $"{1 / deltaTime:.000}";
			fpsNextUpdateTime = DateTime.Now + TimeSpan.FromSeconds(0.5);
		}
	}
}