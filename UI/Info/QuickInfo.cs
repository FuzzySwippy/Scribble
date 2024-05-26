using System;
using Godot;
using Scribble.Application;

namespace Scribble.UI;

public partial class QuickInfo : Node
{
	[Export] private float messageDurationSeconds = 15;

	private Label Label { get; set; }
	private bool HasMessage { get; set; }
	private DateTime LastMessageTime { get; set; }

	public override void _Ready()
	{
		Global.QuickInfo = this;
		Label = GetChild<Label>(0);
	}

	public override void _Process(double delta)
	{
		if (!HasMessage)
			return;

		if ((DateTime.Now - LastMessageTime).TotalSeconds >= messageDurationSeconds)
		{
			Label.Text = "";
			HasMessage = false;
		}
	}

	public void Set(string message)
	{
		Label.Text = message.Replace(System.Environment.NewLine, " ");
		HasMessage = true;
		LastMessageTime = DateTime.Now;
	}
}
