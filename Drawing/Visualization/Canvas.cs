using System.Collections.Generic;
using Godot;
using Scribble.Application;
using ScribbleLib.Input;

namespace Scribble.Drawing.Visualization;

/// <summary>
/// Manages the layer and pixel data of a drawing
/// </summary>
public partial class Canvas : Control
{
	#region Constants
	public const int PixelsPerChunk = 128;
	public static readonly Vector2I DefaultChunkSize = new(PixelsPerChunk, PixelsPerChunk);
	#endregion

	private CanvasData data;

	public Vector2I Size => data.size;
	public Vector2I ChunkRange => data.chunkRange;
	public Vector2I EndChunkSizes => data.endChunkSizes;
	public bool HasXEndChunks => data.hasXEndChunks;
	public bool HasYEndChunks => data.hasYEndChunks;

	public List<Layer> Layers => data.layers;

	private Background background;
	private CanvasLayer imageLayer;
	private CanvasLayer workingLayer;
	private CanvasLayer selectionLayer;

	public override void _Ready()
	{
		Global.Canvas = this;
		GetLayers();
	}

	public override void _Process(double delta)
	{
		if (data == null && Keyboard.IsPressed(Key.C))
		{
			Init(Temp.CanvasSize);
		}

		if (data != null && Keyboard.IsPressed(Key.I))
		{
			Scale *= (float)delta;
		}

		if (data != null && Keyboard.IsPressed(Key.K))
		{
			Scale *= -(float)delta;
		}

		if (data != null && Keyboard.IsPressed(Key.J))
		{
			Position += Vector2.One * (float)delta * 100;
		}

		if (data != null && Keyboard.IsPressed(Key.L))
		{
			Position -= Vector2.One * (float)delta * 100;
		}
	}

	private void GetLayers()
	{
		background = GetChild<Background>(0);
		imageLayer = GetChild<CanvasLayer>(1);
		workingLayer = GetChild<CanvasLayer>(2);
		selectionLayer = GetChild<CanvasLayer>(3);
	}

	public void Init(Vector2I size)
	{
		if (data != null)
			throw new System.Exception("Canvas already initialized");

		data = new(size);
		background.Initialize();
		imageLayer.Initialize();
		workingLayer.Initialize();
		selectionLayer.Initialize();

		Position = Global.Spacer.GlobalPosition;
		//Scale = Vector2.One * 5f;
	}

	#region Pixels
	public void ImagePixelUpdated(Vector2I pos)
	{
		imageLayer.DirtyPixels.Set(pos);
		imageLayer.DirtyChunks.Set(pos / DefaultChunkSize);
	}

	public Color GetImagePixel(Vector2I pos)
	{
		Color color = new();
		for (int l = Layers.Count - 1; l >= 0; l--)
			color = Layers[l].GetPixel(pos).Blend(color);

		return color;
	}

	public void SetWorkingPixel(Vector2I pos, Color color)
	{
		data.workingColors[pos.X, pos.Y] = color;
		workingLayer.DirtyPixels.Set(pos);
		workingLayer.DirtyChunks.Set(pos / DefaultChunkSize);
	}

	public Color GetWorkingPixel(Vector2I pos) => data.workingColors[pos.X, pos.Y];

	public void SetSelectedPixel(Vector2I pos, bool selected)
	{
		data.selectedPixels[pos.X, pos.Y] = selected;
		selectionLayer.DirtyPixels.Set(pos);
		selectionLayer.DirtyChunks.Set(pos / DefaultChunkSize);
	}

	public bool GetSelectedPixel(Vector2I pos) => data.selectedPixels[pos.X, pos.Y];
	#endregion

	/*#region Drawing Tools
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
	#endregion*/

	public void Cleanup()
	{
		if (data == null)
			return;

		background.Cleanup();
		imageLayer.Cleanup();
		workingLayer.Cleanup();
		selectionLayer.Cleanup();

		data = null;
	}
}
