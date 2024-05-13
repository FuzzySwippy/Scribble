using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Scribble.Application;

namespace Scribble.Drawing.Visualization;

/// <summary>
/// Used for canvas data visualization
/// </summary>
public partial class CanvasLayer : Node
{
	private bool initialized = false;

	private Canvas canvas;
	private Vector2I size;
	private Chunk[,] chunks;
	private Vector2I chunkRange;
	private Vector2I endChunkSizes;

	public Dirty DirtyPixels { get; private set; }
	public Dirty DirtyChunks { get; private set; }

	#region Initialization
	public void Initialize()
	{
		if (initialized)
			return;

		initialized = true;
		SetFieldsFromCanvas();
		GenerateChunks();
	}

	private void SetFieldsFromCanvas()
	{
		canvas = Global.Canvas;
		size = canvas.Size;
		chunkRange = canvas.ChunkRange;
		endChunkSizes = canvas.EndChunkSizes;

		DirtyPixels = new(size);
		DirtyChunks = new(chunkRange);
	}

	private void GenerateChunks()
	{
		chunks = new Chunk[chunkRange.X, chunkRange.Y];

		Vector2I chunkSize;
		for (int x = 0; x < chunkRange.X; x++)
		{
			for (int y = 0; y < chunkRange.Y; y++)
			{
				chunkSize = Canvas.DefaultChunkSize;
				if (x == chunkRange.X - 1)
					chunkSize.X = endChunkSizes.X;
				if (y == chunkRange.Y - 1)
					chunkSize.Y = endChunkSizes.Y;

				chunks[x, y] = new Chunk(chunkSize, new Vector2I(x, y) * Canvas.PixelsPerChunk, canvas, this);
			}
		}
	}
	#endregion

	public override void _Process(double delta)
	{
		if (!initialized || !DirtyPixels.IsDirty)
			return;

		Update();
	}

	private readonly List<Task> tasks = new();
	private void Update()
	{
		DirtyChunks.Iterate(pos => tasks.Add(Task.Run(chunks[pos.X, pos.Y].Update)), true);

		Task.WaitAll(tasks.ToArray());
		tasks.Clear();

		DirtyPixels.Clear();
		DirtyChunks.Clear();
	}

	public void Cleanup()
	{
		initialized = false;

		foreach (Chunk chunk in chunks)
			chunk.Dispose();
		chunks = null;

		chunkRange = Vector2I.Zero;
		endChunkSizes = Vector2I.Zero;
		canvas = null;

		DirtyPixels = null;
		DirtyChunks = null;
	}
}
