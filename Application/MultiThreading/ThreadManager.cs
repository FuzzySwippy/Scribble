using System;
using System.Collections.Generic;

namespace Scribble.Application.MultiThreading;

public class ThreadManager
{
	private Dictionary<string, RunnerThread> Threads { get; } = new();

	public void AddThread(string name, Action action)
	{
		if (Threads.ContainsKey(name))
			throw new ArgumentException($"Thread with name {name} already exists");

		Threads.Add(name, new RunnerThread(name, action));
	}

	public void StopThread(string name)
	{
		if (!Threads.TryGetValue(name, out RunnerThread thread))
			throw new ArgumentException($"Thread with name {name} does not exist");

		thread.Stop();
		Threads.Remove(name);
	}

	public void StopAllThreads()
	{
		foreach (var thread in Threads.Values)
			thread.Stop();

		Threads.Clear();
	}
}
