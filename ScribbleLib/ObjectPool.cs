
using System.Collections.Generic;
using Godot;

namespace Scribble.ScribbleLib;

public class ObjectPool<T> where T : Node
{
	public Node Parent { get; }
	public PackedScene Prefab { get; }
	public int GrowSize { get; }

	private Stack<T> Pool { get; } = new();

	public ObjectPool(Node parent, PackedScene prefab, int initialSize = 0, int growSize = 1)
	{
		if (parent == null)
			throw new System.ArgumentNullException(nameof(parent));
		if (prefab == null)
			throw new System.ArgumentNullException(nameof(prefab));
		if (growSize < 1)
			throw new System.ArgumentException("Grow size must be greater than 0", nameof(growSize));

		Parent = parent;
		Prefab = prefab;
		GrowSize = growSize;

		if (initialSize > 0)
			CreateObjects(initialSize);
	}

	private void CreateObjects(int count)
	{
		for (int i = 0; i < count; i++)
		{
			T obj = Prefab.Instantiate<T>();
			Pool.Push(obj);
			Parent.AddChild(obj);
		}
	}

	public T Get()
	{
		if (Pool.Count == 0)
			CreateObjects(GrowSize);

		return Pool.Pop();
	}

	public void Return(T obj) =>
		Pool.Push(obj);
}
