using System;
using System.Collections.Generic;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.UI.Info;

public partial class Notifications : Control
{
	[Export] private float StateChangeSpeed { get; set; } = 1000;
	[Export] private float DisplayTimeSeconds { get; set; } = 5;

	private Queue<string> Queue { get; } = new();

	private Label Label { get; set; }
	private Button CloseButton { get; set; }

	private Vector2 shownPos;
	private Vector2 closedPos;

	private NotificationState State { get; set; } = NotificationState.Open;
	private DateTime OpenTime { get; set; }

	public override void _Ready()
	{
		Global.Notifications = this;

		Label = this.GetGrandChild<Label>(3);
		CloseButton = this.GetGrandChild(2).GetChild<Button>(1);

		shownPos = Position;
		closedPos = new(shownPos.X, -Size.Y * 5);

		CloseButton.Pressed += Close;
		InstantClose();

		Main.WindowSizeChanged += UpdatePositions;
	}

	private void UpdatePositions()
	{
		Position = new(Main.ViewportRect.Size.X / 2 - Size.X / 2, Position.Y);
		shownPos = new(Main.ViewportRect.Size.X / 2 - Size.X / 2, shownPos.Y);
		closedPos = new(shownPos.X, closedPos.Y);
	}

	private void InstantClose()
	{
		Position = closedPos;
		State = NotificationState.Closed;
		Label.Text = string.Empty;
		Size = new(0, Size.Y);
	}

	private void Open(string text)
	{
		Label.Text = text;
		Size = new(0, Size.Y);
		UpdatePositions();
		State = NotificationState.Opening;
	}
	private void Close() => State = NotificationState.Closing;

	public override void _Process(double delta)
	{
		switch (State)
		{
			case NotificationState.Opening:
				Position = Position.MoveToward(shownPos, StateChangeSpeed * (float)delta);
				if (Position.DistanceTo(shownPos) <= 1)
				{
					State = NotificationState.Open;
					OpenTime = DateTime.Now;
					Position = shownPos;
				}
				break;
			case NotificationState.Closing:
				Position = Position.MoveToward(closedPos, StateChangeSpeed * (float)delta);
				if (Position.DistanceTo(closedPos) <= 1)
				{
					State = NotificationState.Closed;
					Size = new(0, Size.Y);
					Position = closedPos;
				}
				break;
			case NotificationState.Open:
				if ((DateTime.Now - OpenTime).TotalSeconds >= DisplayTimeSeconds)
					Close();
				break;
			case NotificationState.Closed:
				if (Queue.Count > 0)
					Open(Queue.Dequeue());
				break;
		}
	}

	public void Enqueue(string text) => Queue.Enqueue(text);
}
