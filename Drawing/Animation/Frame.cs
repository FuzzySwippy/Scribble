using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Drawing.Tools;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Serialization;
using Scribble.UI;

namespace Scribble.Drawing;

public class Frame
{
	private Canvas Canvas { get; }
	private ulong CurrentFrameId => Canvas.Animation.CurrentFrame.Id;

	public ulong Id { get; }
	public Vector2I Size { get; private set; }

	public List<Layer> Layers { get; } = [];

	private int currentLayerIndex = 0;
	public int CurrentLayerIndex
	{
		get => currentLayerIndex;
		set
		{
			currentLayerIndex = value;
			Canvas.UpdateEntireCanvas();
		}
	}

	public Layer CurrentLayer => Layers[CurrentLayerIndex];

	//Preview
	private Image PreviewImage { get; set; }
	public ImageTexture Preview { get; set; }
	public bool PreviewNeedsUpdate { get; set; }

	public Frame(Canvas canvas, Vector2I size, Color[,] colors)
	{
		Canvas = canvas;
		Size = size;
		Id = GetID();

		AddLayer(new Layer(canvas, this, colors));

		CreatePreview(FlattenImage().ToByteArray());
	}

	public Frame(Canvas canvas, Vector2I size, BackgroundType backgroundType = Canvas.DefaultBackgroundType)
	{
		Canvas = canvas;
		Size = size;
		Id = GetID();

		NewLayer(backgroundType, -1, false);

		CreatePreview(FlattenImage().ToByteArray());
	}

	public Frame(Canvas canvas, Vector2I size, Layer[] layers)
	{
		Canvas = canvas;
		Size = size;
		Id = GetID();

		if (layers != null && layers.Length > 0)
			Layers.AddRange(layers);
		else
			NewLayer(-1, false);

		CreatePreview(FlattenImage().ToByteArray());
	}

	/// <summary>
	/// Duplicate a frame
	/// </summary>
	/// <param name="frame">Frame to duplicate</param>
	public Frame(Frame frame, bool withUniqueId)
	{
		Canvas = frame.Canvas;
		Size = frame.Size;

		if (withUniqueId)
			Id = GetID();
		else
			Id = frame.Id;

		CurrentLayerIndex = frame.CurrentLayerIndex;

		foreach (Layer layer in frame.Layers)
			Layers.Add(new(layer));

		CreatePreview(FlattenImage().ToByteArray());
	}

	#region Serialization
	internal Frame(Canvas canvas, byte[] data)
	{
		Canvas = canvas;

		Deserializer deserializer = new(data);

		Id = (ulong)deserializer.DeserializedObjects["id"].Value;
		Size = (Vector2I)deserializer.DeserializedObjects["size"].Value;

		Layer[] layers = new Layer[(int)deserializer.DeserializedObjects["layer_count"].Value];
		for (int i = 0; i < layers.Length; i++)
			layers[i] = new Layer((byte[])deserializer.DeserializedObjects[$"layer_{i}"].Value);
		Layers.AddRange(layers);

		CreatePreview(FlattenImage().ToByteArray());
	}

	internal byte[] Serialize()
	{
		Serializer serializer = new();

		serializer.Write(Id, "id");
		serializer.Write(Size, "size");

		serializer.Write(Layers.Count, "layer_count");
		for (int i = 0; i < Layers.Count; i++)
			serializer.Write(Layers[i].Serialize(), $"layer_{i}");

		return serializer.Finalize();
	}
	#endregion

	private ulong GetID()
	{
		ulong id = (ulong)Global.Random.NextInt64();
		while (Global.Canvas.Animation?.Frames.Any(f => f.Id == id) ?? false)
			id = (ulong)Global.Random.NextInt64();
		return id;
	}

	#region Preview
	private void CreatePreview(byte[] colorData)
	{
		PreviewImage?.Dispose();
		Preview?.Dispose();

		PreviewImage = Image.CreateFromData(
			Size.X, Size.Y, false,
			Image.Format.Rgba8, colorData);
		Preview = ImageTexture.CreateFromImage(PreviewImage);
	}

	public void UpdatePreview()
	{
		PreviewImage?.SetData(Size.X, Size.Y, false,
			Image.Format.Rgba8, FlattenImage().ToByteArray());
		Preview?.Update(PreviewImage);
	}
	#endregion

	#region Flatten
	public void FlattenLayers(Vector2I position, Vector2I size)
	{
		for (int x = position.X; x < position.X + size.X; x++)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{
				Canvas.FlattenedColors[x, y] = new(0, 0, 0, 0);
				for (int l = Layers.Count - 1; l >= -1; l--)
				{
					if (l == -1)
					{
						Canvas.FlattenedColors[x, y] = Layer.BlendColors(Canvas.SelectionOverlay.GetPixel(x, y),
						Canvas.FlattenedColors[x, y]);

						Canvas.FlattenedColors[x, y] = Layer.BlendColors(Canvas.EffectAreaOverlay.GetPixel(x, y),
						Canvas.FlattenedColors[x, y]);
						continue;
					}

					if (!Layers[l].Visible)
						continue;

					if (CurrentLayerIndex == l)
					{
						if (Canvas.Drawing.ToolType != DrawingToolType.SelectionMove ||
							!Canvas.Selection.IsSelectedPixel(new(x, y)) ||
							!((SelectionMoveTool)Canvas.Drawing.DrawingTool).MovingSelection)
							Canvas.FlattenedColors[x, y] = Layer.BlendColors(
								Layers[CurrentLayerIndex].GetPixel(x, y), Canvas.FlattenedColors[x, y]);
					}
					else
						Canvas.FlattenedColors[x, y] = Layer.BlendColors(Layers[l].GetPixel(x, y),
							Canvas.FlattenedColors[x, y]);
				}
			}
		}
	}

	public Color GetFlattenedPixel(int x, int y)
	{
		Color color = new(0, 0, 0, 0);
		for (int l = Layers.Count - 1; l >= 0; l--)
		{
			if (!Layers[l].Visible)
				continue;

			color = Layer.BlendColors(Layers[l].GetPixel(x, y), color);
		}
		return color;
	}

	public Color[,] FlattenImage()
	{
		Color[,] colors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
			for (int y = 0; y < Size.Y; y++)
				colors[x, y] = GetFlattenedPixel(x, y);
		return colors;
	}
	#endregion

	#region Layers
	public void AddLayer(Layer layer) =>
		Layers.Add(layer);

	public void NewLayer(int index, bool recordHistory = true) =>
		NewLayer(BackgroundType.Transparent, index, recordHistory);

	public void NewLayer(BackgroundType backgroundType = BackgroundType.Transparent,
		int index = -1, bool recordHistory = true)
	{
		Layer layer = new(Canvas, this, backgroundType);
		int insertIndex = index < 0 ? CurrentLayerIndex : index;

		if (recordHistory)
			Canvas.History.AddAction(new LayerCreatedHistoryAction(CurrentFrameId, insertIndex));

		Layers.Insert(insertIndex, layer);

		if (Canvas.Animation.Frames.Count > 0)
		{
			Global.LayerEditor.UpdateLayerList();
			Canvas.UpdateEntireCanvas();
		}
	}

	public void SelectLayer(ulong layerId)
	{
		int index = GetLayerIndex(layerId);
		if (index == -1)
			return;

		CurrentLayerIndex = index;
		Global.LayerEditor.UpdateLayerList();
	}

	public void RestoreLayer(Layer layer, int index)
	{
		Layers.Insert(index, layer);
		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	/// <summary>
	/// Used for undo/redo layer move
	/// </summary>
	public void SetLayerIndex(int layerIndex, int newIndex)
	{
		Layer layer = Layers[layerIndex];

		Layers.RemoveAt(layerIndex);
		Layers.Insert(newIndex, layer);

		CurrentLayerIndex = newIndex;
		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void MoveLayerUp(int index)
	{
		ulong selectedID = CurrentLayer.Id;

		int insertIndex = index - 1;
		if (insertIndex < 0)
			insertIndex = Layers.Count - 1;

		Canvas.History.AddAction(new LayerMovedHistoryAction(CurrentFrameId, index, insertIndex));

		Layer layer = Layers[index];
		Layers.RemoveAt(index);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void MoveLayerDown(int index)
	{
		ulong selectedID = CurrentLayer.Id;

		int insertIndex = index + 1;
		if (insertIndex >= Layers.Count)
			insertIndex = 0;

		Canvas.History.AddAction(new LayerMovedHistoryAction(CurrentFrameId, index, insertIndex));

		Layer layer = Layers[index];
		Layers.RemoveAt(index);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void MergeDown(int index, bool recordHistory = true)
	{
		if (index == Layers.Count - 1)
			return;

		Layer layer = Layers[index];
		Layers.RemoveAt(index);

		ulong selectedID = CurrentLayer.Id;

		if (recordHistory)
			Canvas.History.AddAction(new LayerMergedHistoryAction(CurrentFrameId, layer, index, Layers[index]));

		Layers[index].MergeUnder(layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		if (CurrentLayerIndex == -1)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void SetLayerVisibility(ulong id, bool visible, bool recordHistory = true) =>
		SetLayerVisibility(GetLayerIndex(id), visible, recordHistory);

	public void SetLayerVisibility(int index, bool visible, bool recordHistory = true)
	{
		Layer layer = Layers[index];

		if (recordHistory && layer.Visible != visible)
			Canvas.History.AddAction(new LayerVisibilityChangedHistoryAction(
				index, layer.Visible, visible));

		layer.Visible = visible;
		Canvas.UpdateEntireCanvas();
	}

	public void DuplicateLayer(int index, bool recordHistory = true)
	{
		ulong selectedID = CurrentLayer.Id;

		Layer layer = new(Layers[index]);
		Layers.Insert(index, layer);

		if (recordHistory)
			Canvas.History.AddAction(new LayerDuplicatedHistoryAction(CurrentFrameId, index));

		CurrentLayerIndex = GetLayerIndex(selectedID);

		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void DeleteLayer(int index, bool recordHistory = true)
	{
		if (Layers.Count == 1)
			return;

		if (recordHistory)
			Canvas.History.AddAction(new LayerDeletedHistoryAction(CurrentFrameId, Layers[index], index));

		Layers.RemoveAt(index);
		if (CurrentLayerIndex >= Layers.Count)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		Canvas.UpdateEntireCanvas();
	}

	public void SetLayerOpacity(int index, float opacity, bool recordHistory = true)
	{
		if (recordHistory)
			Canvas.History.AddAction(new LayerOpacityChangedHistoryAction(
				index, Layers[index].Opacity, opacity));

		Layers[index].Opacity = opacity;
		Layers[index].PreviewNeedsUpdate = true;
		Canvas.UpdateEntireCanvas();
	}

	public int GetLayerIndex(ulong id) =>
		Layers.FindIndex(l => l.Id == id);

	public void SetLayerName(int index, string name, bool recordHistory = true)
	{
		if (recordHistory)
			Canvas.History.AddAction(new LayerNameChangedHistoryAction(
				index, Layers[index].Name, name));

		Layers[index].Name = name;
	}
	#endregion

	#region ImageOperations
	public void FlipVertically()
	{
		foreach (Layer layer in Layers)
			layer.FlipVertically();

		CreatePreview(FlattenImage().ToByteArray());
	}

	public void FlipHorizontally()
	{
		foreach (Layer layer in Layers)
			layer.FlipHorizontally();

		CreatePreview(FlattenImage().ToByteArray());
	}

	public void RotateClockwise()
	{
		Size = new(Size.Y, Size.X);

		foreach (Layer layer in Layers)
			layer.RotateClockwise();

		CreatePreview(FlattenImage().ToByteArray());
	}

	public void RotateCounterClockwise()
	{
		Size = new(Size.Y, Size.X);

		foreach (Layer layer in Layers)
			layer.RotateCounterClockwise();

		CreatePreview(FlattenImage().ToByteArray());
	}

	/// <summary>
	/// Resizes the frame
	/// </summary>
	/// <returns>The old frame data</returns>
	public Frame Resize(Vector2I newSize, ResizeType type)
	{
		Frame oldFrame = new(this, false);

		Size = newSize;

		//Resize layers
		foreach (Layer layer in Layers)
			layer.Resize(newSize, type);

		CreatePreview(FlattenImage().ToByteArray());

		return oldFrame;
	}

	/// <summary>
	/// Crops all the frame layers to the specified bounds
	/// </summary>
	/// <param name="bound">Bounds to crop to</param>
	/// <returns>The old frame copy</returns>
	public Frame CropToBounds(Rect2I bound)
	{
		Frame oldFrame = new(this, false);

		Size = Canvas.Size;

		foreach (Layer layer in Layers)
			layer.CropToBounds(bound);

		CreatePreview(FlattenImage().ToByteArray());

		return oldFrame;
	}
	#endregion
}
