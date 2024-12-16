using System;
using System.Threading;
using Godot;

namespace Scribble.Application.MultiThreading;

public class RunnerThread
{
	private Thread Thread { get; set; }
	private Action Action { get; set; }
	private bool Running { get; set; } = true;

	public string Name { get; }
	public bool Active => Thread.IsAlive || Running;

	public RunnerThread(string name, Action action)
	{
		ArgumentException.ThrowIfNullOrEmpty(nameof(name));
		ArgumentException.ThrowIfNullOrEmpty(nameof(action));

		GD.Print($"Creating thread '{name}'");

		Name = name;
		Action = action;

		Thread = new Thread(Run);
		Thread.Start();
	}

	private void Run()
	{
		GD.Print($"Thread '{Name}' started");
		while (Running)
		{
			Action?.Invoke();
			Thread.Sleep(1);
		}
		GD.Print($"Thread '{Name}' stopped");
	}

	public void Stop() => Running = false;
}
