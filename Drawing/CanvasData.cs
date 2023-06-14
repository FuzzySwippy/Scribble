using System.Collections.Generic;
using Godot;

namespace Scribble.Drawing;

public class CanvasData
{
	public readonly Vector2I size;

	//Chunk data
	public readonly Vector2I chunkRange;
	public readonly Vector2I endChunkSizes;
	public readonly bool hasXEndChunks;
	public readonly bool hasYEndChunks;

	//Color data
	public readonly List<Layer> layers = new();

	public readonly Color[,] colors;

	/// <summary>
	/// Temporary color data to be committed to the <see cref="colors"/> array.
	/// </summary>
	public readonly Color[,] workingColors;
	public readonly bool[,] selectedPixels;

	public CanvasData(Vector2I size)
	{
		this.size = size;
		colors = new Color[size.X, size.Y];
		workingColors = new Color[size.X, size.Y];
		selectedPixels = new bool[size.X, size.Y];

		chunkRange = new(Mathf.CeilToInt((float)size.X / Canvas.PixelsPerChunk), Mathf.CeilToInt((float)size.Y / Canvas.PixelsPerChunk));
		endChunkSizes = new(size.X % Canvas.PixelsPerChunk, size.Y % Canvas.PixelsPerChunk);
		hasXEndChunks = endChunkSizes.X != 0;
		hasYEndChunks = endChunkSizes.Y != 0;
	}
}
