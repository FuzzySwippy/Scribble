using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.Application.Versioning;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Formats;
using Scribble.ScribbleLib.Serialization;
using Scribble.UI;

using Version = Scribble.Application.Versioning.Version;

namespace Scribble.Drawing;
public partial class Canvas : Control
{
	public const float BaseScale = 2048;

	public const int ChunkSize = 64;
	public const int DefaultResolution = 64;
	public const int MaxResolution = 1024;
	public const int MinResolution = 1;

	private const int BGResolutionMult = 2;

	public const BackgroundType DefaultBackgroundType = BackgroundType.Transparent;

	private Vector2I StandardChunkSize { get; } = new(ChunkSize, ChunkSize);

	private Artist Artist { get; set; }

	//Nodes
	public Control ChunkParent { get; private set; }
	public TextureRect Background { get; private set; }

	//Values
	public static Vector2 SizeInWorld { get; private set; }
	public new Vector2I Size { get; set; }
	public Vector2 TargetScale { get; private set; }
	public Vector2 PixelSize => TargetScale;

	//Chunks
	private ObjectPool<CanvasChunk> ChunkPool { get; set; }
	private CanvasChunk[,] Chunks { get; set; }
	private Vector2I ChunkGridSize { get; set; }
	private Vector2I EndChunkSize { get; set; }
	private bool HasChunkUpdates { get; set; }

	private Vector2 oldWindowSize;

	public DrawingController Drawing { get; private set; }
	public History History { get; private set; }

	public Animation Animation { get; private set; }

	public Color[,] FlattenedColors { get; private set; }

	//Layers
	public List<Layer> Layers => Animation?.CurrentFrame?.Layers ?? null;
	public Layer EffectAreaOverlay { get; private set; }
	public Layer SelectionOverlay { get; private set; }

	public int CurrentLayerIndex
	{
		get => Animation.CurrentFrame.CurrentLayerIndex;
		set => Animation.CurrentFrame.CurrentLayerIndex = value;
	}
	public Layer CurrentLayer => Layers?[CurrentLayerIndex] ?? null;

	private DateTime LayerPreviewLastUpdate { get; set; } = DateTime.Now;

	private TimeSpan PreviewUpdateInterval { get; } = TimeSpan.FromMilliseconds(250);

	//Frame
	public Frame CurrentFrame => Animation.CurrentFrame;
	private DateTime FramePreviewLastUpdate { get; set; } = DateTime.Now;

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
				Global.FileDialogs.SetSaveAndExportFileName(
					Path.GetFileName(previousScribbleSavePath)[..^Path.GetExtension(previousScribbleSavePath).Length]);
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

	//Threading
	public object ChunkUpdateThreadLock { get; } = new();

	public override void _Ready()
	{
		ChunkParent = GetChild<Control>(1);
		Background = GetChild<TextureRect>(0);

		ChunkPool = new(ChunkParent, Global.CanvasChunkPrefab, 256);

		Global.FileDialogs.FileSelectedEvent += FileSelected;
		Main.Ready += MainReady;
	}

	private void MainReady() =>
		Global.ThreadManager.AddThread("chunk_update", UpdateChunks);

	public override void _Process(double delta)
	{
		Drawing?.Update();

		UpdateLayerPreviews();
		UpdateFramePreviews();
		AutosaveUpdate();
	}

	private void AutosaveUpdate()
	{
		if (Global.Settings.AutosaveEnabled && HasUnsavedChanges &&
			(DateTime.Now - LastAutoSave).Minutes >=
				Global.Settings.AutosaveIntervalMinutes &&
			!string.IsNullOrEmpty(PreviousScribbleSavePath) &&
			Global.AnimationTimeline.DraggedFrame == null)
		{
			GD.Print($"Autosaving to: {PreviousScribbleSavePath}");
			SaveToPreviousPath();
			LastAutoSave = DateTime.Now;
		}
	}

	public void Init(Artist artist, string[] cmdLineArgs)
	{
		Artist = artist;
		Drawing = new(this, artist);

		if (cmdLineArgs.Length == 0 || !File.Exists(cmdLineArgs[0]))
			CreateNew(DefaultResolution.ToVector2I(), BackgroundType.Transparent);
		else
			LoadDataFromFile(cmdLineArgs[0]);

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
		Background.Scale = TargetScale;
	}

	//Drawing
	public bool PixelInBounds(Vector2I position) =>
		position.X >= 0 && position.Y >= 0 &&
		position.X < Size.X && position.Y < Size.Y;

	private Color GetFlattenedNoOpacityPixel(int x, int y)
	{
		Color color = new(0, 0, 0, 0);
		for (int l = Layers.Count - 1; l >= 0; l--)
		{
			if (!Layers[l].Visible)
				continue;

			color = Layer.BlendColors(Layers[l].GetPixelNoOpacity(x, y), color);
		}
		return color;
	}

	public void UpdateChunkMesh(CanvasChunk chunk)
	{
		chunk.MarkedForUpdate = false;
		Animation.CurrentFrame.FlattenLayers(chunk.PixelPosition, chunk.SizeInPixels);
		chunk.SetColors(FlattenedColors);
		chunk.Update();
	}

	public void UpdateEntireCanvas()
	{
		Chunks?.ForEach(c => c.MarkedForUpdate = true);
		HasChunkUpdates = true;
	}

	#region PixelGettersAndSetters
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

	public bool BlendPixel(Vector2I position, Color color, BlendMode blendType)
	{
		if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
			return false;

		CurrentLayer.BlendPixel(position, color, blendType);

		Chunks[position.X / ChunkSize, position.Y / ChunkSize].MarkedForUpdate = true;
		HasChunkUpdates = true;
		HasUnsavedChanges = true;
		return true;
	}

	public Color GetPixel(Vector2I position) =>
		GetPixelInLayer(position, CurrentLayer);

	public Color GetPixelNoOpacity(Vector2I position) =>
		GetPixelNoOpacityInLayer(position, CurrentLayer);

	public Color GetPixelFlattenedNoOpacity(Vector2I position) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : GetFlattenedNoOpacityPixel(position.X, position.Y);

	public Color GetPixelFlattened(Vector2I position) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : Animation.CurrentFrame.GetFlattenedPixel(position.X, position.Y);

	//In layer
	public bool SetPixelInLayer(Vector2I position, Color color, Layer layer)
	{
		if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
			return false;

		layer.SetPixel(position, color);

		Chunks[position.X / ChunkSize, position.Y / ChunkSize].MarkedForUpdate = true;
		HasChunkUpdates = true;
		HasUnsavedChanges = true;
		return true;
	}

	public Color GetPixelInLayer(Vector2I position, Layer layer) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : layer.GetPixel(position);

	public Color GetPixelNoOpacityInLayer(Vector2I position, Layer layer) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : layer.GetPixelNoOpacity(position);
	#endregion

	#region SelectFrame
	public void SelectFrame(ulong frameId) => Animation.SelectFrame(frameId);

	public void SelectFrameAndLayer(ulong frameId, ulong layerId)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.SelectLayer(layerId);
	}

	public void SelectFrameAndCreateLayer(ulong frameId, int layerIndex, bool recordHistory)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.NewLayer(layerIndex, recordHistory);
	}

	public void SelectFrameAndRestoreLayer(ulong frameId, Layer layer, int index)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.RestoreLayer(layer, index);
	}

	public void SelectFrameAndDeleteLayer(ulong frameId, ulong layerId, bool recordHistory)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.DeleteLayer(layerId, recordHistory);
	}

	public void SelectFrameAndDeleteLayer(ulong frameId, int layerIndex, bool recordHistory)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.DeleteLayer(layerIndex, recordHistory);
	}

	public void SelectFrameAndDuplicateLayer(ulong frameId, int layerIndex, bool recordHistory)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.DuplicateLayer(layerIndex, recordHistory);
	}

	public void SelectFrameAndMergeLayer(ulong frameId, int layerIndex, bool recordHistory)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.MergeDown(layerIndex, recordHistory);
	}

	public void SelectFrameAndMoveLayer(ulong frameId, int fromIndex, int toIndex)
	{
		SelectFrame(frameId);
		Animation.CurrentFrame.SetLayerIndex(fromIndex, toIndex);
	}
	#endregion

	#region ImageOperations
	public void FlipVertically(bool recordHistory = true) =>
		Animation.FlipVertically(recordHistory);

	public void FlipHorizontally(bool recordHistory = true) =>
		Animation.FlipHorizontally(recordHistory);

	public void RotateClockwise(bool recordHistory = true) =>
		Animation.RotateClockwise(recordHistory);

	public void RotateCounterClockwise(bool recordHistory = true) =>
		Animation.RotateCounterClockwise(recordHistory);

	public void Resize(Vector2I newSize, ResizeType type, bool recordHistory = true) =>
		Animation.Resize(newSize, type, recordHistory);

	public void ResizeWithFrameData(Vector2I newSize, FrameHistoryData[] frameHistoryData) =>
		Animation.ResizeWithFrameData(newSize, frameHistoryData);

	public void CropToContent(CropType type, bool recordHistory = true) =>
		Animation.CropToContent(type, recordHistory);

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
		Background.Texture?.Dispose();
		Background.Texture = TextureGenerator.NewBackgroundTexture(Size *
			BGResolutionMult);

		//Disable texture filtering and set background node size
		Background.TextureFilter = TextureFilterEnum.Nearest;
		Background.Size = Size;
	}

	private void Create(Vector2I size, BackgroundType backgroundType, Frame[] frames, bool animationLoop = true, int animationFrameTimeMs = 100)
	{
		Size = size;
		Animation = new(this);
		History = new();
		Global.HistoryList.Update();

		Animation.Loop = animationLoop;
		Animation.FrameTimeMs = animationFrameTimeMs;

		UpdateScale();
		SetBackgroundTexture();


		EffectAreaOverlay = new(this, null, BackgroundType.Transparent);
		SelectionOverlay = new(this, null, BackgroundType.Transparent);

		Selection = new(Size);

		lock (ChunkUpdateThreadLock)
		{
			Animation.SetFrames(frames, backgroundType);
		}

		FlattenedColors = new Color[Size.X, Size.Y];
		GenerateChunks();

		//Position the camera's starting position in the middle of the canvas
		Global.Camera.Position = SizeInWorld / 2;
		UpdateEntireCanvas();
		Global.AnimationTimeline.Update();
		Global.LayerEditor.UpdateLayerList();

		if (Global.Settings.AutosaveEnabled && string.IsNullOrEmpty(PreviousScribbleSavePath))
			LastAutoSave = DateTime.Now;
	}

	public void CreateNew(Vector2I size, BackgroundType backgroundType, bool reportToNotifications = true)
	{
		PreviousScribbleSavePath = null;
		FilePath = null;
		HasUnsavedChanges = false;
		Create(size, backgroundType, null);

		if (reportToNotifications)
			Global.Notifications.Enqueue("New canvas created!");

		Status.Set("canvas_size", Size);
	}

	public void CreateFromData(Vector2I size, Frame[] frames, bool animationLoop = true, int animationFrameTimeMs = 100) =>
		Create(size, DefaultBackgroundType, frames, animationLoop, animationFrameTimeMs);

	public void Recreate(bool setUnsavedChanges)
	{
		EffectAreaOverlay = new(this, null, BackgroundType.Transparent);
		SelectionOverlay = new(this, null, BackgroundType.Transparent);
		Selection = new(Size);
		FlattenedColors = new Color[Size.X, Size.Y];

		GenerateChunks();
		SetBackgroundTexture();
		UpdateScale();
		UpdateEntireCanvas();

		Global.AnimationTimeline.Update();
		Global.LayerEditor.UpdateLayerList();
		if (setUnsavedChanges)
			HasUnsavedChanges = true;

		Status.Set("canvas_size", Size);
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
			OverlayType.EffectArea => [EffectAreaOverlay],
			OverlayType.Selection => [SelectionOverlay],
			OverlayType.All => [EffectAreaOverlay, SelectionOverlay],
			_ => throw new Exception("Invalid overlay type"),
		};

		foreach (Layer overlay in overlays)
		{
			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
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

	public void ClearOverlayPixels(OverlayType type, List<Vector2I> pixels)
	{
		if (pixels == null || pixels.Count == 0)
			return;

		Layer[] overlays = type switch
		{
			OverlayType.EffectArea => [EffectAreaOverlay],
			OverlayType.Selection => [SelectionOverlay],
			OverlayType.All => [EffectAreaOverlay, SelectionOverlay],
			_ => throw new Exception("Invalid overlay type"),
		};

		foreach (Vector2I pos in pixels)
		{
			foreach (Layer overlay in overlays)
			{
				if (pos.X < 0 || pos.Y < 0 || pos.X >= Size.X || pos.Y >= Size.Y)
					continue;

				overlay.SetPixel(pos, new());
				Chunks[pos.X / ChunkSize, pos.Y / ChunkSize].MarkedForUpdate = true;
				HasChunkUpdates = true;
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
		lock (ChunkUpdateThreadLock)
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
	}

	private void UpdateLayerPreviews()
	{
		if (DateTime.Now - LayerPreviewLastUpdate < PreviewUpdateInterval)
			return;

		LayerPreviewLastUpdate = DateTime.Now;

		foreach (Layer layer in Layers)
		{
			if (!layer.PreviewNeedsUpdate)
				continue;

			layer.UpdatePreview();
		}
	}

	private void UpdateFramePreviews()
	{
		if (DateTime.Now - FramePreviewLastUpdate < PreviewUpdateInterval)
			return;

		FramePreviewLastUpdate = DateTime.Now;

		foreach (Frame frame in Animation.Frames)
		{
			if (!frame.PreviewNeedsUpdate)
				continue;

			frame.UpdatePreview();
		}
	}

	private void UpdateChunks()
	{
		lock (ChunkUpdateThreadLock)
		{
			if (!HasChunkUpdates || Chunks == null)
				return;

			DebugInfo.Set("chunk_updates",
			 	$"{Chunks.Count(c => c.MarkedForUpdate)} / {Chunks.Count()}");

			foreach (CanvasChunk chunk in Chunks)
			{
				if (!chunk.MarkedForUpdate)
					continue;

				UpdateChunkMesh(chunk);
			}

			if (CurrentLayer != null)
				CurrentLayer.PreviewNeedsUpdate = true;

			if (CurrentFrame != null)
				CurrentFrame.PreviewNeedsUpdate = true;
		}
	}
	#endregion

	#region Serialization
	private byte[] SerializeToScrbl()
	{
		Serializer serializer = new();

		serializer.Write("Scribble", "format");
		serializer.Write(Global.Version, "version");
		serializer.Write(Size, "size");

		//Animation
		serializer.Write(Animation.Loop, "animation_loop");
		serializer.Write(Animation.FrameTimeMs, "animation_frame_time");

		serializer.Write(Animation.Frames.Count, "frame_count");
		for (int i = 0; i < Animation.Frames.Count; i++)
			serializer.Write(Animation.Frames[i].Serialize(), $"frame_{i}");

		return serializer.Finalize();
	}

	private void DeserializeFromScrbl(byte[] data)
	{
		Frame[] frames;
		bool animationLoop = true;
		int animationFrameTimeMs = 100;

		try
		{
			Deserializer deserializer = new(data);

			//Format
			string format = "Scribble_Old";
			if (deserializer.DeserializedObjects.TryGetValue("format", out DeserializedObject formatObject))
				format = (string)formatObject.Value;
			else
				GD.Print("No format found in file. Loading pre-Alpha_0.4.0 format...");

			//Version
			string versionString = "Unknown";
			if (deserializer.DeserializedObjects.TryGetValue("version", out DeserializedObject versionObject))
				versionString = (string)versionObject.Value;
			else
				GD.Print("No version found in file. Loading pre-Alpha_0.4.0 format...");

			Version version = new();
			if (!Version.TryParse(versionString, out version))
				GD.Print("Failed to parse version string.");

			GD.Print($"Loading Scribble file with format '{format}' and version '{version}'");

			//Size
			Size = (Vector2I)deserializer.DeserializedObjects["size"].Value;
			if (Size.X > MaxResolution || Size.Y > MaxResolution)
				throw new Exception($"Image resolution is too large. Maximum supported resolution is {MaxResolution}x{MaxResolution}");

			//Animation
			if (deserializer.DeserializedObjects.TryGetValue("animation_loop", out DeserializedObject animationLoopObject))
				animationLoop = (bool)animationLoopObject.Value;

			if (deserializer.DeserializedObjects.TryGetValue("animation_frame_time", out DeserializedObject animationFrameTimeObject))
				animationFrameTimeMs = (int)animationFrameTimeObject.Value;

			//Frames And Layers
#pragma warning disable IDE0045 // If statement can be simplified
			if (version.ReleaseType == ReleaseType.Unknown || version < new Version(ReleaseType.Alpha, 0, 5, 0))
				frames = DeserializeScrblAlpha040(deserializer);
			else
				frames = DeserializeScrblAlpha050(deserializer);
#pragma warning restore IDE0045

		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data (The file may be corrupt)", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return;
		}

		CreateFromData(Size, frames, animationLoop, animationFrameTimeMs);
	}

	private Frame[] DeserializeScrblAlpha040(Deserializer deserializer)
	{
		GD.Print("Deserializing frames and layers from pre-Alpha_0.5.0 format...");

		Layer[] layers = new Layer[(int)deserializer.DeserializedObjects["layer_count"].Value];
		for (int i = 0; i < layers.Length; i++)
			layers[i] = new Layer((byte[])deserializer.DeserializedObjects[$"layer_{i}"].Value);

		return [new(this, Size, layers)];
	}

	private Frame[] DeserializeScrblAlpha050(Deserializer deserializer)
	{
		GD.Print("Deserializing frames and layers from Alpha_0.5.0 format...");

		Frame[] frames = new Frame[(int)deserializer.DeserializedObjects["frame_count"].Value];
		for (int i = 0; i < frames.Length; i++)
			frames[i] = new Frame(this, (byte[])deserializer.DeserializedObjects[$"frame_{i}"].Value);

		return frames;
	}

	private Image GetFlattenedImage(Vector2I size)
	{
		Image image = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8,
			Animation.CurrentFrame.FlattenImage().ToByteArray());

		if (size != Size)
			image.Resize(size.X, size.Y, Image.Interpolation.Nearest);

		return image;
	}

	private Image[] GetFlattenedFrames(Vector2I size)
	{
		Image[] images = new Image[Animation.Frames.Count];
		for (int i = 0; i < Animation.Frames.Count; i++)
		{
			Image image = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8,
				Animation.Frames[i].FlattenImage().ToByteArray());

			if (size != Size)
				image.Resize(size.X, size.Y, Image.Interpolation.Nearest);

			images[i] = image;
		}

		return images;
	}

	private byte[] SerializeToFormat(ImageFormat format, Vector2I size) => format switch
	{
		ImageFormat.PNG => GetFlattenedImage(size).SavePngToBuffer(),
		ImageFormat.JPEG => GetFlattenedImage(size).SaveJpgToBuffer(),
		ImageFormat.WEBP => GetFlattenedImage(size).SaveWebpToBuffer(),
		ImageFormat.BMP => new BMP(GetFlattenedImage(size)).Serialize(),
		ImageFormat.GIF =>
			new GIF(GetFlattenedFrames(size), Animation.FrameTimeMs, Animation.Loop, Animation.BlackIsTransparent).Serialize(true),
		ImageFormat.APNG =>
			new APNG(GetFlattenedFrames(size), Animation.FrameTimeMs, Animation.Loop, Animation.BlackIsTransparent).Serialize(true),
		_ => throw new Exception("Unsupported image format"),
	};

	/// <summary>
	/// Deserializes image data from a given format.
	/// </summary>
	/// <param name="data">Image data</param>
	/// <param name="format">Target image format</param>
	/// <returns><see langword="true"/> if the data was deserialized successfully, and <see langword="false"/> otherwise</returns>
	private bool DeserializeImageFromFormat(byte[] data)
	{
		Color[,] colors;

		try
		{
			Image image = new();
			Error error = LoadImageFromData(image, data);

			if (error != Error.Ok)
				throw new Exception($"Error loading image: {error}");
			else if (image.GetWidth() > MaxResolution || image.GetHeight() > MaxResolution)
				throw new Exception($"Image resolution is too large. Maximum supported resolution is {MaxResolution}x{MaxResolution}");
			else if (image.GetWidth() < MinResolution || image.GetHeight() < MinResolution)
				throw new Exception($"Image resolution is too small. Minimum supported resolution is {MinResolution}x{MinResolution}");

			Size = new(image.GetWidth(), image.GetHeight());
			colors = image.GetColorsFromImage();
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return false;
		}

		CreateFromData(Size, [new(this, Size, colors)]);
		return true;
	}

	private Error LoadImageFromData(Image image, byte[] data)
	{
		Func<byte[], Error>[] loaders =
		[
			image.LoadPngFromBuffer,
			image.LoadJpgFromBuffer,
			image.LoadWebpFromBuffer,
			image.LoadBmpFromBuffer,
		];

		Error error = Error.FileUnrecognized;
		foreach (Func<byte[], Error> loader in loaders)
		{
			error = loader(data);
			if (error == Error.Ok)
				return error;
		}

		throw new Exception("Unsupported image format");
	}

	/// <summary>
	/// Deserializes frame data from a given format.
	/// </summary>
	/// <param name="data">Image data</param>
	/// <param name="format">Target image format</param>
	/// <returns><see langword="true"/> if the data was deserialized successfully, and <see langword="false"/> otherwise</returns>
	private bool DeserializeFramesFromFormat(byte[] data)
	{
		List<Frame> frames = [];
		bool loop;
		int frameTimeMs;

		try
		{
			(List<Image> frames, bool loop, int frameTimeMs) animationData =
				LoadFramesFromData(data);

			//Validation
			if (animationData.frames.Count == 0)
				throw new Exception("Image has no frames");

			Vector2I size = animationData.frames[0].GetSize();
			if (size.X > MaxResolution || size.Y > MaxResolution)
				throw new Exception($"Image resolution is too large. Maximum supported resolution is {MaxResolution}x{MaxResolution}");
			else if (size.X < MinResolution || size.Y < MinResolution)
				throw new Exception($"Image resolution is too small. Minimum supported resolution is {MinResolution}x{MinResolution}");

			//Load frames
			Size = size;

			foreach (Image image in animationData.frames)
				frames.Add(new(this, image.GetSize(), image.GetColorsFromImage()));
			loop = animationData.loop;
			frameTimeMs = animationData.frameTimeMs;
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return false;
		}

		CreateFromData(Size, frames.ToArray(), loop, frameTimeMs);
		return true;
	}

	private (List<Image> frames, bool loop, int frameTimeMs) LoadFramesFromData(byte[] data)
	{
		Func<byte[], (List<Image> frames, bool loop, int frameTimeMs)?>[] loaders =
		[
			GIF.LoadFramesFromBuffer,
			APNG.LoadFramesFromBuffer,
		];

		foreach (Func<byte[], (List<Image> frames, bool loop, int frameTimeMs)?> loader in loaders)
		{
			(List<Image> frames, bool loop, int frameTimeMs)? animationData = loader(data);
			if (animationData != null) //Error loading if null
				return animationData.Value;
		}

		throw new Exception("Unsupported image format");
	}
	#endregion

	#region DataSavingAndLoading
	private void FileSelected(FileDialogType type, string file, object[] additionalData)
	{
		if (type == FileDialogType.Open)
			LoadDataFromFile(file);
		else
			SaveDataToFile(file, additionalData, type);
	}

	private void LoadDataFromFile(string file)
	{
		try
		{
			if (!File.Exists(file))
				throw new FileNotFoundException("File not found", file);

			string extension = Path.GetExtension(file);
			ImageFormat format = ImageFormatParser.FileExtensionToImageFormat(extension);

			if (format == ImageFormat.Invalid)
				throw new FileLoadException("Invalid file type", file);

			byte[] data = File.ReadAllBytes(file);
			bool deserializationError = false;

			switch (format)
			{
				case ImageFormat.SCRIBBLE:
					PreviousScribbleSavePath = file;
					DeserializeFromScrbl(data);
					break;
				case ImageFormat.PNG:
				case ImageFormat.JPEG:
				case ImageFormat.WEBP:
				case ImageFormat.BMP:
					PreviousScribbleSavePath = null;
					deserializationError = !DeserializeImageFromFormat(data);
					break;
				case ImageFormat.GIF:
				case ImageFormat.APNG:
					PreviousScribbleSavePath = null;
					deserializationError = !DeserializeFramesFromFormat(data);
					break;
			}

			FilePath = file;
			SaveDirectoryPath = file;

			HasUnsavedChanges = false;
			if (!deserializationError)
				Global.Notifications.Enqueue($"File '{Path.GetFileName(file)}' loaded successfully!");
			Status.Set("canvas_size", Size);

			//Show animation timeline if more than one frame
			if (Animation.Frames.Count > 1)
				Global.AnimationTimeline.Show();
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while loading file", ex);
			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
		}
	}

	public void SaveDataToFile(string file, object[] additionalData = null, FileDialogType dialogType = FileDialogType.Save)
	{
		if (File.Exists(file))
			File.Delete(file);

		string extension = Path.GetExtension(file);
		if (string.IsNullOrWhiteSpace(extension))
		{
			GD.Print("No file extension found. Adding default extension...");
			extension = dialogType is FileDialogType.Save ? ".scrbl" : (Animation.Frames.Count > 1 ? ".apng" : ".png");
			file += extension;
		}

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
		Global.Notifications.Enqueue($"File '{Path.GetFileName(file)}' saved successfully!");
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
