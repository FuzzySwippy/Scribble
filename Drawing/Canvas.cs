using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Drawing.Tools;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Serialization;
using Scribble.UI;

namespace Scribble.Drawing;
public partial class Canvas : Node2D
{
	public const float BaseScale = 2048;

	public const int ChunkSize = 64;
	public const int DefaultResolution = 64;
	public const int MaxResolution = 1024;
	public const int MinResolution = 1;

	private const int BGResolutionMult = 2;

	private Vector2I StandardChunkSize { get; } = new(ChunkSize, ChunkSize);

	private Artist Artist { get; set; }

	//Nodes
	public Node2D ChunkParent { get; private set; }
	private Panel BackgroundPanel { get; set; }

	//Values
	public static Vector2 SizeInWorld { get; private set; }
	public Vector2I Size { get; private set; }
	public Vector2 TargetScale { get; private set; }
	public Vector2 PixelSize => TargetScale;

	//Chunks
	private ObjectPool<CanvasChunk> ChunkPool { get; set; }
	private CanvasChunk[,] Chunks { get; set; }
	private Vector2I ChunkGridSize { get; set; }
	private Vector2I EndChunkSize { get; set; }
	private Color[,] FlattenedColors { get; set; }
	private bool HasChunkUpdates { get; set; }

	private Vector2 oldWindowSize;

	public DrawingController Drawing { get; private set; }
	public History History { get; private set; }

	//Layers
	public List<Layer> Layers { get; } = new();
	public Layer EffectAreaOverlay { get; private set; }
	public Layer SelectionOverlay { get; private set; }

	public ulong NextLayerID { get; set; }
	private int currentLayerIndex;
	public int CurrentLayerIndex
	{
		get => currentLayerIndex;
		set
		{
			currentLayerIndex = value;
			UpdateEntireCanvas();
		}
	}
	public Layer CurrentLayer => Layers[CurrentLayerIndex];
	private DateTime LayerPreviewLastUpdate { get; set; } = DateTime.Now;
	private TimeSpan LayerUpdateInterval { get; } = TimeSpan.FromMilliseconds(250);

	//Image manipulation
	public Selection Selection { get; private set; }

	//Saving and loading
	private string previousScribbleSavePath;
	private string PreviousScribbleSavePath
	{
		get => previousScribbleSavePath;
		set
		{
			previousScribbleSavePath = value;

			if (string.IsNullOrEmpty(previousScribbleSavePath))
				Global.FileDialogs.SetSaveAndExportFileName("");
			else
			{
				Global.FileDialogs.SetSaveAndExportFileName(
					Path.GetFileName(previousScribbleSavePath)[..^Path.GetExtension(previousScribbleSavePath).Length]);
			}
		}
	}

	public bool HasUnsavedChanges { get; set; }

	//File dialog properties
	private string filePath;
	public string FilePath
	{
		get => string.IsNullOrWhiteSpace(filePath) ? SaveDirectoryPath : filePath;
		private set => filePath = value == null ? "" : value[..^Path.GetExtension(value).Length];
	}

	private string saveDirectoryPath;
	public string SaveDirectoryPath
	{
		get => saveDirectoryPath;
		set => saveDirectoryPath = $"{Path.GetDirectoryName(value)}{(string.IsNullOrWhiteSpace(value) ? "" : Path.DirectorySeparatorChar)}";
	}

	//Dynamic properties
	private static Vector2 ScreenScaleMultiplier
	{
		get
		{
			Vector2 multiplier = Main.Window.Size.ToVector2() / Main.BaseWindowSize;
			return multiplier.X > multiplier.Y ?
				new(multiplier.Y, multiplier.Y) :
				new(multiplier.X, multiplier.X);
		}
	}

	//Autosave
	private DateTime LastAutoSave { get; set; }

	//Events
	public event Action Initialized;

	public override void _Ready()
	{
		ChunkParent = GetChild<Node2D>(1);
		BackgroundPanel = GetChild<Panel>(0);

		ChunkPool = new(ChunkParent, Global.CanvasChunkPrefab, 256);

		Global.FileDialogs.FileSelectedEvent += FileSelected;
	}

	public override void _Process(double delta)
	{
		Drawing?.Update();

		UpdateChunks();
		UpdateLayerPreviews();
		AutosaveUpdate();
	}

	private void AutosaveUpdate()
	{
		if (Global.Settings.AutosaveEnabled && HasUnsavedChanges &&
			(DateTime.Now - LastAutoSave).Minutes >=
				Global.Settings.AutosaveIntervalMinutes &&
			!string.IsNullOrEmpty(Global.Canvas.PreviousScribbleSavePath))
		{
			SaveToPreviousPath();
			LastAutoSave = DateTime.Now;
		}
	}

	public void Init(Vector2I size, Artist artist)
	{
		Artist = artist;
		Drawing = new(this, artist);

		CreateNew(size, BackgroundType.Transparent);

		Main.WindowSizeChanged += UpdateScale;

		Initialized?.Invoke();
	}

	private void UpdateScale()
	{
		TargetScale = Vector2.One * (BaseScale / (Size.X > Size.Y ? Size.X : Size.Y)) *
			ScreenScaleMultiplier;
		SizeInWorld = PixelSize * Size;

		//Update camera position based on the window size change
		Global.Camera.Position *= Main.Window.Size / oldWindowSize;
		oldWindowSize = Main.Window.Size;

		//Update node scales
		ChunkParent.Scale = TargetScale;
		BackgroundPanel.Scale = TargetScale;
	}

	//Drawing
	public bool PixelInBounds(Vector2I position) =>
		position.X >= 0 && position.Y >= 0 &&
		position.X < Size.X && position.Y < Size.Y;

	private void FlattenLayers(Vector2I position, Vector2I size)
	{
		for (int x = position.X; x < position.X + size.X; x++)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{
				FlattenedColors[x, y] = new(0, 0, 0, 0);
				for (int l = Layers.Count - 1; l >= -1; l--)
				{
					if (l == -1)
					{
						FlattenedColors[x, y] = Layer.BlendColors(SelectionOverlay.GetPixel(x, y),
						FlattenedColors[x, y]);

						FlattenedColors[x, y] = Layer.BlendColors(EffectAreaOverlay.GetPixel(x, y),
						FlattenedColors[x, y]);
						continue;
					}

					if (!Layers[l].Visible)
						continue;

					if (CurrentLayerIndex == l)
					{
						if (Drawing.ToolType != DrawingToolType.SelectionMove ||
							!Selection.IsSelectedPixel(new(x, y)) ||
							!((SelectionMoveTool)Drawing.DrawingTool).MovingSelection)
							FlattenedColors[x, y] = Layer.BlendColors(
								Layers[CurrentLayerIndex].GetPixel(x, y), FlattenedColors[x, y]);
					}
					else
						FlattenedColors[x, y] = Layer.BlendColors(Layers[l].GetPixel(x, y),
							FlattenedColors[x, y]);
				}
			}
		}
	}

	private Color[,] FlattenImage()
	{
		Color[,] colors = new Color[Size.X, Size.Y];
		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				for (int l = Layers.Count - 1; l >= 0; l--)
				{
					if (!Layers[l].Visible)
						continue;

					colors[x, y] = Layer.BlendColors(Layers[l].GetPixel(x, y), colors[x, y]);
				}
			}
		}
		return colors;
	}

	public void UpdateChunkMesh(CanvasChunk chunk)
	{
		FlattenLayers(chunk.PixelPosition, chunk.SizeInPixels);
		chunk.SetColors(FlattenedColors);
		chunk.UpdateMesh();
	}

	public void UpdateEntireCanvas()
	{
		Chunks?.ForEach(c => c.MarkedForUpdate = true);
		HasChunkUpdates = true;
	}

	public bool SetPixel(Vector2I position, Color color)
	{
		if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
			return false;
		CurrentLayer.SetPixel(position, color);

		Chunks[position.X / ChunkSize, position.Y / ChunkSize].MarkedForUpdate = true;
		HasChunkUpdates = true;
		HasUnsavedChanges = true;
		return true;
	}

	public Color GetPixel(Vector2I position) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : CurrentLayer.GetPixel(position);

	#region ImageOperations
	public void FlipVertically(bool recordHistory = true)
	{
		foreach (Layer layer in Layers)
			layer.FlipVertically();
		UpdateEntireCanvas();
		HasUnsavedChanges = true;

		if (recordHistory)
			History.AddAction(new FlippedVerticallyHistoryAction());
	}

	public void FlipHorizontally(bool recordHistory = true)
	{
		foreach (Layer layer in Layers)
			layer.FlipHorizontally();
		UpdateEntireCanvas();
		HasUnsavedChanges = true;

		if (recordHistory)
			History.AddAction(new FlippedHorizontallyHistoryAction());
	}

	public void RotateClockwise(bool recordHistory = true)
	{
		foreach (Layer layer in Layers)
			layer.RotateClockwise();

		Size = new(Size.Y, Size.X);
		Recreate(true);

		if (recordHistory)
			History.AddAction(new RotatedClockwiseHistoryAction());
	}

	public void RotateCounterClockwise(bool recordHistory = true)
	{
		foreach (Layer layer in Layers)
			layer.RotateCounterClockwise();

		Size = new(Size.Y, Size.X);
		Recreate(true);

		if (recordHistory)
			History.AddAction(new RotatedCounterClockwiseHistoryAction());
	}

	public void Resize(Vector2I newSize, ResizeType type, bool recordHistory = true)
	{
		if (newSize.X == Size.X && newSize.Y == Size.Y)
			return;

		if (recordHistory)
			Selection.Clear();

		Vector2I oldSize = Size;
		Size = newSize;

		//Resize layers
		List<LayerHistoryData> layerHistoryData = new();
		foreach (Layer layer in Layers)
			layerHistoryData.Add(new(layer.ID, layer.Resize(newSize, type)));

		Recreate(true);

		if (recordHistory)
			History.AddAction(
				new CanvasResizedHistoryAction(oldSize, newSize, type, layerHistoryData.ToArray()));
	}

	public void ResizeWithLayerData(Vector2I newSize, LayerHistoryData[] layerHistoryData)
	{
		if (newSize.X == Size.X && newSize.Y == Size.Y)
			return;

		Vector2I oldSize = Size;
		Size = newSize;

		//Resize layers
		foreach (Layer layer in Layers)
		{
			LayerHistoryData historyData = layerHistoryData.AsReadOnly()
				.First(l => l.LayerId == layer.ID);
			Color[,] colors = historyData?.Colors ?? new Color[newSize.X, newSize.Y];

			layer.ResizeWithColorData(newSize, colors);
		}

		Recreate(false);
	}

	public void CropToContent(CropType type, bool recordHistory = true)
	{
		FlattenImage().CropToContent(type, out Rect2I bounds);

		if (bounds.Size == Size || bounds.Size.X == 0 || bounds.Size.Y == 0)
		{
			WindowManager.ShowModal("Canvas is already cropped to content", ModalOptions.Ok);
			return;
		}

		if (recordHistory)
			Selection.Clear();

		Vector2I oldSize = Size;
		Size = bounds.Size;

		//Resize layers
		List<LayerHistoryData> layerHistoryData = new();
		foreach (Layer layer in Layers)
			layerHistoryData.Add(new(layer.ID, layer.CropToBounds(bounds)));

		Recreate(true);

		if (recordHistory)
			History.AddAction(
				new CanvasCroppedHistoryAction(oldSize, type, layerHistoryData.ToArray()));
	}

	public void Cut()
	{
		bool layer = Selection.Copy(true);
		Global.QuickInfo.Set($"{(layer ? "Layer" : "Selection")} cut to clipboard");
	}

	public void Copy()
	{
		bool layer = Selection.Copy(false);
		Global.QuickInfo.Set($"{(layer ? "Layer" : "Selection")} copied to clipboard");
	}

	public void Paste()
	{
		if (Selection.Paste())
			Global.QuickInfo.Set("Pasted from clipboard");
		else
			Global.QuickInfo.Set("Nothing to paste");
	}

	/// <summary>
	/// Used for history action (does not record history)
	/// </summary>
	public void Paste(Vector2I mousePos, Image image) => Selection.Paste(mousePos, image);

	public void Undo() => History.Undo();
	public void Redo() => History.Redo();
	#endregion

	#region New
	private void SetBackgroundTexture()
	{
		Global.BackgroundStyle.Texture = TextureGenerator.NewBackgroundTexture(Size *
			BGResolutionMult);

		//Disable texture filtering and set background node size
		BackgroundPanel.TextureFilter = TextureFilterEnum.Nearest;
		BackgroundPanel.Size = Size;
	}

	private void Create(Vector2I size, BackgroundType? backgroundType, Layer[] layers)
	{
		Size = size;
		History = new();
		Global.HistoryList.Update();

		UpdateScale();
		SetBackgroundTexture();


		EffectAreaOverlay = new(this, BackgroundType.Transparent);
		SelectionOverlay = new(this, BackgroundType.Transparent);

		Selection = new(Size);

		CurrentLayerIndex = 0;
		Layers.Clear();
		if (layers != null)
			Layers.AddRange(layers);
		else
			NewLayer(backgroundType.Value, -1, false);

		FlattenedColors = new Color[Size.X, Size.Y];
		GenerateChunks();

		//Position the camera's starting position in the middle of the canvas
		Global.Camera.Position = SizeInWorld / 2;
		UpdateEntireCanvas();
		Global.LayerEditor.UpdateLayerList();

		if (Global.Settings.AutosaveEnabled && string.IsNullOrEmpty(PreviousScribbleSavePath))
			LastAutoSave = DateTime.Now;
	}

	public void CreateNew(Vector2I size, BackgroundType backgroundType, bool reportToQuickInfo = true)
	{
		PreviousScribbleSavePath = null;
		FilePath = null;
		HasUnsavedChanges = false;
		Create(size, backgroundType, null);

		if (reportToQuickInfo)
			Global.QuickInfo.Set("New canvas created!");

		Status.Set("canvas_size", Size);
	}

	public void CreateFromData(Vector2I size, Layer[] layers) =>
		Create(size, null, layers);

	private void Recreate(bool setUnsavedChanges)
	{
		EffectAreaOverlay = new(this, BackgroundType.Transparent);
		SelectionOverlay = new(this, BackgroundType.Transparent);
		Selection = new(Size);
		FlattenedColors = new Color[Size.X, Size.Y];

		GenerateChunks();
		SetBackgroundTexture();
		UpdateScale();
		UpdateEntireCanvas();

		Global.LayerEditor.UpdateLayerList();
		if (setUnsavedChanges)
			HasUnsavedChanges = true;
	}
	#endregion

	#region Layers
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
		UpdateEntireCanvas();
	}

	public void NewLayer(int index, bool recordHistory = true) =>
		NewLayer(BackgroundType.Transparent, index, recordHistory);

	public void NewLayer(BackgroundType backgroundType = BackgroundType.Transparent,
		int index = -1, bool recordHistory = true)
	{
		Layer layer = new(this, backgroundType);
		int insertIndex = index < 0 ? CurrentLayerIndex : index;

		if (recordHistory)
			History.AddAction(new LayerCreatedHistoryAction(insertIndex));

		Layers.Insert(insertIndex, layer);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
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
		UpdateEntireCanvas();
	}

	public void MoveLayerUp(int index)
	{
		ulong selectedID = CurrentLayer.ID;

		int insertIndex = index - 1;
		if (insertIndex < 0)
			insertIndex = Layers.Count - 1;

		History.AddAction(new LayerMovedHistoryAction(index, insertIndex));

		Layer layer = Layers[index];
		Layers.RemoveAt(index);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MoveLayerDown(int index)
	{
		ulong selectedID = CurrentLayer.ID;

		int insertIndex = index + 1;
		if (insertIndex >= Layers.Count)
			insertIndex = 0;

		History.AddAction(new LayerMovedHistoryAction(index, insertIndex));

		Layer layer = Layers[index];
		Layers.RemoveAt(index);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MergeDown(int index, bool recordHistory = true)
	{
		if (index == Layers.Count - 1)
			return;

		Layer layer = Layers[index];
		Layers.RemoveAt(index);

		ulong selectedID = CurrentLayer.ID;

		if (recordHistory)
			History.AddAction(new LayerMergedHistoryAction(layer, index, Layers[index]));

		Layers[index].MergeUnder(layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		if (CurrentLayerIndex == -1)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerVisibility(ulong id, bool visible, bool recordHistory = true) =>
		SetLayerVisibility(GetLayerIndex(id), visible, recordHistory);

	public void SetLayerVisibility(int index, bool visible, bool recordHistory = true)
	{
		Layer layer = Layers[index];

		if (recordHistory && layer.Visible != visible)
			History.AddAction(new LayerVisibilityChangedHistoryAction(
				index, layer.Visible, visible));

		layer.Visible = visible;
		UpdateEntireCanvas();
	}

	public void DuplicateLayer(int index, bool recordHistory = true)
	{
		ulong selectedID = CurrentLayer.ID;

		Layer layer = new(Layers[index]);
		Layers.Insert(index, layer);

		if (recordHistory)
			History.AddAction(new LayerDuplicatedHistoryAction(index));

		CurrentLayerIndex = GetLayerIndex(selectedID);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void DeleteLayer(int index, bool recordHistory = true)
	{
		if (Layers.Count == 1)
			return;

		if (recordHistory)
			History.AddAction(new LayerDeletedHistoryAction(Layers[index], index));

		Layers.RemoveAt(index);
		if (CurrentLayerIndex >= Layers.Count)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerOpacity(int index, float opacity, bool recordHistory = true)
	{
		if (recordHistory)
			History.AddAction(new LayerOpacityChangedHistoryAction(
				index, Layers[index].Opacity, opacity));

		Layers[index].Opacity = opacity;
		Layers[index].PreviewNeedsUpdate = true;
		UpdateEntireCanvas();
	}

	public int GetLayerIndex(ulong id) =>
		Layers.FindIndex(l => l.ID == id);

	public void SetLayerName(int index, string name, bool recordHistory = true)
	{
		if (recordHistory)
			History.AddAction(new LayerNameChangedHistoryAction(
				index, Layers[index].Name, name));

		Layers[index].Name = name;
	}
	#endregion

	#region Overlays
	public void SetOverlayPixel(Vector2I position, Color underColor, OverlayType type)
	{
		Layer overlay = type switch
		{
			OverlayType.EffectArea => EffectAreaOverlay,
			OverlayType.EffectAreaAlt => EffectAreaOverlay,
			OverlayType.Selection => SelectionOverlay,
			OverlayType.NoSelection => SelectionOverlay,
			_ => throw new Exception("Invalid overlay type"),
		};

		if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
			return;

		if (overlay.GetPixel(position) != new Color())
			return;

		overlay.SetPixel(position, underColor.Blend(type is OverlayType.NoSelection or OverlayType.EffectAreaAlt ?
			Artist.RestrictedAreaOverlayColor : Artist.EffectAreaOverlayColor));

		Chunks[position.X / ChunkSize, position.Y / ChunkSize].MarkedForUpdate = true;
		HasChunkUpdates = true;
	}

	public void ClearOverlay(OverlayType type)
	{
		Layer[] overlays = type switch
		{
			OverlayType.EffectArea => new Layer[] { EffectAreaOverlay },
			OverlayType.Selection => new Layer[] { SelectionOverlay },
			OverlayType.All => new Layer[] { EffectAreaOverlay, SelectionOverlay },
			_ => throw new Exception("Invalid overlay type"),
		};

		for (int x = 0; x < Size.X; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				foreach (Layer overlay in overlays)
				{
					if (overlay.GetPixel(new(x, y)) == new Color())
						continue;

					overlay.SetPixel(new(x, y), new());
					Chunks[x / ChunkSize, y / ChunkSize].MarkedForUpdate = true;
					HasChunkUpdates = true;
				}
			}
		}
	}
	#endregion

	#region Chunks
	private void ClearChunks()
	{
		if (Chunks == null)
			return;

		HasChunkUpdates = false;
		foreach (CanvasChunk chunk in Chunks)
		{
			chunk.Clear();
			ChunkPool.Return(chunk);
		}

		Chunks = null;
	}

	private void GenerateChunks()
	{
		ClearChunks();

		ChunkGridSize = new(Mathf.CeilToInt((float)Size.X / ChunkSize), Mathf.CeilToInt((float)Size.Y / ChunkSize));
		EndChunkSize = new(Size.X % ChunkSize, Size.Y % ChunkSize);

		Chunks = new CanvasChunk[ChunkGridSize.X, ChunkGridSize.Y];

		for (int x = 0; x < ChunkGridSize.X; x++)
		{
			for (int y = 0; y < ChunkGridSize.Y; y++)
			{
				CanvasChunk chunk = ChunkPool.Get();
				Vector2I size = StandardChunkSize;
				if (x == ChunkGridSize.X - 1 && EndChunkSize.X > 0)
					size.X = EndChunkSize.X;
				if (y == ChunkGridSize.Y - 1 && EndChunkSize.Y > 0)
					size.Y = EndChunkSize.Y;

				chunk.Init(new(x * ChunkSize, y * ChunkSize), size);
				Chunks[x, y] = chunk;
			}
		}
	}

	private void UpdateLayerPreviews()
	{
		if (DateTime.Now - LayerPreviewLastUpdate < LayerUpdateInterval)
			return;

		LayerPreviewLastUpdate = DateTime.Now;

		foreach (Layer layer in Layers)
		{
			if (!layer.PreviewNeedsUpdate)
				continue;

			layer.UpdatePreview();
		}
	}

	private void UpdateChunks()
	{
		if (!HasChunkUpdates)
			return;

		DebugInfo.Set("chunk_updates",
			$"{Chunks.Count(c => c.MarkedForUpdate)} / {Chunks.Count()}");

		foreach (CanvasChunk chunk in Chunks)
		{
			if (!chunk.MarkedForUpdate)
				continue;

			UpdateChunkMesh(chunk);

			if (Main.FrameTimeMs > Main.TargetFrameTimeMs)
				break;
		}

		CurrentLayer.PreviewNeedsUpdate = true;
	}
	#endregion

	#region Serialization
	private byte[] SerializeToScrbl()
	{
		Serializer serializer = new();

		serializer.Write(Size, "size");
		serializer.Write(Layers.Count, "layer_count");
		for (int i = 0; i < Layers.Count; i++)
			serializer.Write(Layers[i].Serialize(), $"layer_{i}");

		return serializer.Finalize();
	}

	private void DeserializeFromScrbl(byte[] data)
	{
		Layer[] layers;

		try
		{
			Deserializer deserializer = new(data);

			Size = (Vector2I)deserializer.DeserializedObjects["size"].Value;
			if (Size.X > MaxResolution || Size.Y > MaxResolution)
				throw new Exception($"Image resolution is too large. Maximum supported resolution is {MaxResolution}x{MaxResolution}");

			layers = new Layer[(int)deserializer.DeserializedObjects["layer_count"].Value];
			for (int i = 0; i < layers.Length; i++)
				layers[i] = new Layer((byte[])deserializer.DeserializedObjects[$"layer_{i}"].Value);
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data (The file may be corrupt)", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return;
		}

		CreateFromData(Size, layers);
	}

	private Image GetFlattenedImage(Vector2I size)
	{
		Image image = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8,
			FlattenImage().ToByteArray());

		if (size != Size)
			image.Resize(size.X, size.Y, Image.Interpolation.Nearest);

		return image;
	}

	private byte[] SerializeToFormat(ImageFormat format, Vector2I size) => format switch
	{
		ImageFormat.PNG => GetFlattenedImage(size).SavePngToBuffer(),
		ImageFormat.JPEG => GetFlattenedImage(size).SaveJpgToBuffer(),
		ImageFormat.WEBP => GetFlattenedImage(size).SaveWebpToBuffer(),
		_ => throw new Exception("Unsupported image format"),
	};

	private void DeserializeFromFormat(byte[] data, ImageFormat format)
	{
		Layer[] layers;

		try
		{
			Image image = new();
			switch (format)
			{
				case ImageFormat.PNG:
					image.LoadPngFromBuffer(data);
					break;
				case ImageFormat.JPEG:
					image.LoadJpgFromBuffer(data);
					break;
				case ImageFormat.WEBP:
					image.LoadWebpFromBuffer(data);
					break;
				default:
					throw new Exception("Unsupported image format");
			}

			if (image.GetWidth() > MaxResolution || image.GetHeight() > MaxResolution)
				throw new Exception($"Image resolution is too large. Maximum supported resolution is {MaxResolution}x{MaxResolution}");

			Size = new(image.GetWidth(), image.GetHeight());
			layers = new Layer[] { new(this, image.GetColorsFromImage()) };
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return;
		}

		CreateFromData(Size, layers);
	}
	#endregion

	#region DataSavingAndLoading
	private void FileSelected(FileDialogType type, string file, object[] additionalData)
	{
		if (type == FileDialogType.Open)
			LoadDataFromFile(file);
		else
			SaveDataToFile(file, additionalData);
	}

	private void LoadDataFromFile(string file)
	{
		if (!File.Exists(file))
			throw new FileNotFoundException("File not found", file);

		string extension = Path.GetExtension(file);
		ImageFormat format = ImageFormatParser.FileExtensionToImageFormat(extension);

		if (format == ImageFormat.Invalid)
			throw new FileLoadException("Invalid file type", file);

		byte[] data = File.ReadAllBytes(file);

		switch (format)
		{
			case ImageFormat.SCRIBBLE:
				PreviousScribbleSavePath = file;
				DeserializeFromScrbl(data);
				break;
			case ImageFormat.PNG:
			case ImageFormat.JPEG:
			case ImageFormat.WEBP:
				PreviousScribbleSavePath = null;
				DeserializeFromFormat(data, format);
				break;
		}

		FilePath = file;
		SaveDirectoryPath = file;

		HasUnsavedChanges = false;
		Global.QuickInfo.Set($"File '{Path.GetFileName(file)}' loaded successfully!");
		Status.Set("canvas_size", Size);
	}

	public void SaveDataToFile(string file, object[] additionalData = null)
	{
		if (File.Exists(file))
			File.Delete(file);

		string extension = Path.GetExtension(file);
		ImageFormat format = ImageFormatParser.FileExtensionToImageFormat(extension);
		if (format == ImageFormat.Invalid)
			throw new FileLoadException("Invalid file type", file);
		else if (format == ImageFormat.SCRIBBLE)
			PreviousScribbleSavePath = file;

		FilePath = file;
		SaveDirectoryPath = file;

		//Additional data used to get the export size
		Vector2I exportSize = Size;
		if (additionalData != null && additionalData.Length > 0 && additionalData[0] is Vector2I size)
			exportSize = size;

		byte[] data = format switch
		{
			ImageFormat.SCRIBBLE => SerializeToScrbl(),
			_ => SerializeToFormat(format, exportSize),
		};

		using FileStream stream = new(file, FileMode.Create, System.IO.FileAccess.Write);
		stream.Write(data, 0, data.Length);
		stream.Flush();
		stream.Close();

		HasUnsavedChanges = false;
		Global.QuickInfo.Set($"File '{Path.GetFileName(file)}' saved successfully!");
	}

	/// <summary>
	/// Returns true if the file was saved successfully and false if there was no previous save path or an error occurred.
	/// </summary>
	/// <returns></returns>
	public static bool SaveToPreviousPath()
	{
		if (string.IsNullOrEmpty(Global.Canvas.PreviousScribbleSavePath))
		{
			FileDialogs.Show(FileDialogType.Save);
			return false;
		}

		try
		{
			Global.Canvas.SaveDataToFile(Global.Canvas.PreviousScribbleSavePath);
		}
		catch (Exception ex)
		{
			Modal errorModal = Main.ReportError("An error occurred while saving file", ex);
			errorModal.Hidden += () => FileDialogs.Show(FileDialogType.Save);
			return false;
		}

		if (Global.Settings.AutosaveEnabled)
			Global.Canvas.LastAutoSave = DateTime.Now;

		Global.InteractionBlocker.Hide();
		return true;
	}
	#endregion
}
