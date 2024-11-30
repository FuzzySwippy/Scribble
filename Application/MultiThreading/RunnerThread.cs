using System;
using System.Threading;

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

		Name = name;

		Thread = new Thread(Run);
		Thread.Start();
	}

	private void Run()
	{
		while (Running)
		{
			Action?.Invoke();
			Thread.Sleep(1);
		}
	}

	public void Stop()
	{
		Running = false;
	}
}
