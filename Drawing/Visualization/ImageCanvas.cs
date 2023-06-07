using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Scribble.Drawing.Visualization;

/// <summary>
/// Used for canvas data visualization
/// </summary>
public partial class ImageCanvas : Node
{
	Canvas canvas;

	//Visualization
	bool initialized = false;
	Chunk[,] chunks;
	Vector2I chunkRange;
	Vector2I endChunkSizes;

	Vector2I Resolution => canvas.size;

	public Texture2D BackgroundTexture { get; private set; }
	public Texture2D XEndBackgroundTexture { get; private set; }
	public Texture2D YEndBackgroundTexture { get; private set; }
	public Texture2D XYEndBackgroundTexture { get; private set; }

	#region Initialization
	public override void _Ready() => Global.ImageCanvas = this;

	public void Initialize(Canvas canvas)
	{
		this.canvas = canvas;
		GenerateChunks();
	}

	void GenerateBackgroundTextures()
	{
		BackgroundTexture ??= SetBackgroundTexture(new(Canvas.PixelsPerChunk, Canvas.PixelsPerChunk));

		XEndBackgroundTexture = SetBackgroundTexture(new(endChunkSizes.X, Canvas.PixelsPerChunk));
		YEndBackgroundTexture = SetBackgroundTexture(new(Canvas.PixelsPerChunk, endChunkSizes.Y));
		XYEndBackgroundTexture = SetBackgroundTexture(endChunkSizes);
	}

	Texture2D SetBackgroundTexture(Vector2I size) =>
		TextureGenerator.NewBackgroundTexture(size * Global.Settings.Canvas.BG_ResolutionMult);

	void GenerateChunks()
	{
		if (initialized)
			Cleanup();

		chunkRange = canvas.chunkRange;
		endChunkSizes = new(Resolution.X % Canvas.PixelsPerChunk, Resolution.Y % Canvas.PixelsPerChunk);

		chunks = new Chunk[chunkRange.X, chunkRange.Y];

		Vector2I chunkSize;
		Texture2D backgroundTexture;
		for (int x = 0; x < chunkRange.X; x++)
		{
			for (int y = 0; y < chunkRange.Y; y++)
			{
				chunkSize = Canvas.DefaultChunkSize;
				backgroundTexture = BackgroundTexture;
				if (x == chunkRange.X - 1)
				{
					chunkSize.X = endChunkSizes.X;
					backgroundTexture = XEndBackgroundTexture;
				}
				if (y == chunkRange.Y - 1)
				{
					chunkSize.Y = endChunkSizes.Y;
					backgroundTexture = backgroundTexture == XEndBackgroundTexture ? XYEndBackgroundTexture : YEndBackgroundTexture;
				}

				chunks[x, y] = new Chunk(chunkSize, new Vector2I(x, y) * Canvas.PixelsPerChunk, canvas, this, backgroundTexture);
			}
		}
	}
	#endregion

	public override void _Process(double delta)
	{
		if (!initialized || !canvas.IsDirty)
			return;

		UpdateChunks();
		canvas.SetClean();
	}

	readonly List<Task> tasks = new();
	void UpdateChunks()
	{
		for (int x = 0; x < chunkRange.X; x++)
		{
			for (int y = 0; y < chunkRange.Y; y++)
			{
				if (canvas.dirtyChunks[x, y] || canvas.AllDirty)
				{
					canvas.dirtyChunks[x, y] = false;

					//Local x and y. Set to x and y so they can be used in the lambda and not be changed by the loop
					int lx = x, ly = y;
					tasks.Add(Task.Run(chunks[lx, ly].Update));
				}
			}
		}

		Task.WaitAll(tasks.ToArray());
		tasks.Clear();
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
	}
}
