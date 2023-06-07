using System;
using System.Collections.Generic;
using Godot;

namespace Scribble.Drawing;

/// <summary>
/// Manages the layer and pixel data of a drawing
/// </summary>
public class Canvas : IDisposable
{
	public const int PixelsPerChunk = 128;
	public static readonly Vector2I DefaultChunkSize = new(PixelsPerChunk, PixelsPerChunk);


	public readonly Vector2I size;
	public readonly Vector2I chunkRange;

	readonly List<Layer> layers = new();
	readonly Color[,] colors;
	readonly bool[,] dirty;

	public readonly bool[,] dirtyChunks;

	/// <summary>
	/// If true, all pixels need to be recalculated
	/// </summary>
	public bool AllDirty => allDirty;
	bool allDirty = false;

	/// <summary>
	/// If true, the canvas has been modified and needs to be redrawn
	/// </summary>
	/// <value></value>
	public bool IsDirty => isDirty || allDirty;
	bool isDirty = false;

	public Canvas(Vector2I size)
	{
		this.size = size;
		colors = new Color[size.X, size.Y];
		dirty = new bool[size.X, size.Y];

		chunkRange = new(Mathf.CeilToInt((float)size.X / PixelsPerChunk), Mathf.CeilToInt((float)size.Y / PixelsPerChunk));
		dirtyChunks = new bool[chunkRange.X, chunkRange.Y];
	}

	#region Pixel Update
	/// <summary>
	/// Recalculates the pixel at the given coordinates if it is dirty
	/// </summary>
	/// <returns>The color of the pixel if it was recalculated, null if it was not</returns>
	public Color? RecalculatePixel(int x, int y)
	{
		if (!dirty[x, y] && !allDirty)
			return null;

		dirty[x, y] = false;
		colors[x, y] = new();
		for (int l = layers.Count - 1; l >= 0; l--)
			colors[x, y] = layers[l].GetPixel(x, y).Blend(colors[x, y]);

		return colors[x, y];
	}

	public void SetDirty(int x, int y)
	{
		dirty[x, y] = true;
		dirtyChunks[x / PixelsPerChunk, y / PixelsPerChunk] = true;
		isDirty = true;
	}

	public void SetAllDirty() => allDirty = true;

	public void SetClean()
	{
		isDirty = false;
		allDirty = false;
	}
	#endregion

	#region Drawing Tools
	public void SetPixelOnAllLayers(int x, int y, Color color)
	{
		for (int l = layers.Count - 1; l >= 0; l--)
			layers[l].SetColor(x, y, color);
	}

	public Layer GetTopNonAlphaZeroLayer(int x, int y)
	{
		for (int i = 0; i < layers.Count; i++)
			if (layers[i].GetPixel(x, y).A > 0)
				return layers[i];
		return null;
	}

	public Layer GetTopAlphaOneLayer(int x, int y)
	{
		for (int i = 0; i < layers.Count; i++)
			if (layers[i].GetPixel(x, y).A == 1)
				return layers[i];
		return null;
	}
	#endregion

	public void Dispose() => Global.ImageCanvas.Cleanup();
}
