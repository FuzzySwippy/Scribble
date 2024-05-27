using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.ScribbleLib.Serialization;
using Scribble.UI;

namespace Scribble.Drawing;
public partial class Canvas : Node2D
{
	public const float BaseScale = 2048;

	public const int ChunkSize = 32;
	public const int DefaultResolution = 64;
	public const int MaxResolution = 1024;
	public const int MinResolution = 1;

	private Vector2I StandardChunkSize { get; } = new(ChunkSize, ChunkSize);

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
	private bool UpdateAllChunks { get; set; }

	private Artist artist;
	private Vector2 oldWindowSize;
	private readonly Dictionary<MouseCombination, QuickPencilType> mouseColorMap = new()
	{
		{ new (MouseButton.Left), QuickPencilType.Primary },
		{ new (MouseButton.Right), QuickPencilType.Secondary },
		{ new (MouseButton.Left, KeyModifierMask.MaskCtrl), QuickPencilType.AltPrimary },
		{ new (MouseButton.Right, KeyModifierMask.MaskCtrl), QuickPencilType.AltSecondary },
	};

	//Pixel
	private Vector2I oldMousePixelPos = Vector2I.One * -1;
	private Vector2I frameMousePixelPos;
	public Vector2I MousePixelPos => frameMousePixelPos;

	//Layers
	public List<Layer> Layers { get; } = new();

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

	//Saving and loading
	private string previousSavePath;
	private string PreviousSavePath
	{
		get => previousSavePath;
		set
		{
			previousSavePath = value;

			if (string.IsNullOrEmpty(previousSavePath))
				Global.FileDialogs.SetSaveAndExportFileName("");
			else
			{
				Global.FileDialogs.SetSaveAndExportFileName(
					Path.GetFileName(previousSavePath)[..^Path.GetExtension(previousSavePath).Length]);
			}
		}
	}
	public bool HasUnsavedChanges { get; set; }

	//Dynamic properties
	private static Vector2 ScreenScaleMultiplier
	{
		get
		{
			Vector2 multiplier = Main.Window.Size.ToVector2() / Main.BaseWindowSize;
			return multiplier.X > multiplier.Y ? new(multiplier.Y, multiplier.Y) : new(multiplier.X, multiplier.X);
		}
	}

	private Brush Brush => artist.Brush;

	public override void _Ready()
	{
		ChunkParent = GetChild<Node2D>(1);
		BackgroundPanel = GetChild<Panel>(0);

		ChunkPool = new(ChunkParent, Global.CanvasChunkPrefab, 256);

		Global.FileDialogs.FileSelectedEvent += FileSelected;
	}

	public override void _Process(double delta)
	{
		frameMousePixelPos = (Mouse.GlobalPosition / PixelSize).ToVector2I();

		if (oldMousePixelPos != MousePixelPos)
		{
			if (Spacer.MouseInBounds)
			{
				foreach (MouseCombination combination in mouseColorMap.Keys)
					if (Mouse.IsPressed(combination))
						Brush.Line(MousePixelPos, oldMousePixelPos, Brush.GetQuickPencilColor(mouseColorMap[combination]).GodotColor);
			}
			oldMousePixelPos = MousePixelPos;
		}

		Status.Set("pixel_pos", MousePixelPos);

		UpdateChunks();
		UpdateLayerPreviews();
	}

	public void Init(Vector2I size, Artist artist)
	{
		this.artist = artist;

		CreateNew(size, BackgroundType.Transparent);
		Mouse.ButtonDown += MouseDown;
		Main.WindowSizeChanged += UpdateScale;

		Status.Set("canvas_size", Size);
	}

	private void MouseDown(MouseCombination combination, Vector2 position)
	{
		if (!Spacer.MouseInBounds)
			return;

		if (mouseColorMap.TryGetValue(combination, out QuickPencilType value))
			Brush.Pencil(MousePixelPos, Brush.GetQuickPencilColor(value).GodotColor);
	}

	private void UpdateScale()
	{
		TargetScale = Vector2.One * (BaseScale / (Size.X > Size.Y ? Size.X : Size.Y)) * ScreenScaleMultiplier;
		SizeInWorld = PixelSize * Size;

		//Update camera position based on the window size change
		Global.Camera.Position *= Main.Window.Size / oldWindowSize;
		oldWindowSize = Main.Window.Size;

		//Update node scales
		ChunkParent.Scale = TargetScale;
		BackgroundPanel.Scale = TargetScale;
	}

	//Drawing
	private void FlattenLayers(Vector2I position, Vector2I size)
	{
		for (int x = position.X; x < position.X + size.X; x++)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{
				FlattenedColors[x, y] = new(0, 0, 0, 0);
				for (int l = Layers.Count - 1; l >= 0; l--)
				{
					if (!Layers[l].Visible && CurrentLayerIndex != l)
						continue;

					FlattenedColors[x, y] = Layer.BlendColors(Layers[l].GetPixel(x, y), FlattenedColors[x, y]);
				}
			}
		}
	}

	public void UpdateChunkMesh(CanvasChunk chunk)
	{
		FlattenLayers(chunk.PixelPosition, chunk.SizeInPixels);
		chunk.SetColors(FlattenedColors);
		chunk.UpdateMesh();
	}

	public void UpdateEntireCanvas()
	{
		UpdateAllChunks = true;
		HasChunkUpdates = true;
	}

	public void SetPixel(Vector2I position, Color color)
	{
		if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
			return;
		CurrentLayer.SetPixel(position, color);

		Chunks[position.X / ChunkSize, position.Y / ChunkSize].MarkedForUpdate = true;
		HasChunkUpdates = true;
		HasUnsavedChanges = true;
	}

	public Color GetPixel(Vector2I position) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : CurrentLayer.GetPixel(position);

	#region New
	private void Create(Vector2I size, BackgroundType? backgroundType, Layer[] layers)
	{
		Size = size;
		UpdateScale();
		SetBackgroundTexture();

		CurrentLayerIndex = 0;
		Layers.Clear();
		if (layers != null)
			Layers.AddRange(layers);
		else
			NewLayer(backgroundType.Value);

		FlattenedColors = new Color[Size.X, Size.Y];
		GenerateChunks();

		//Position the camera's starting position in the middle of the canvas
		Global.Camera.Position = SizeInWorld / 2;
		UpdateEntireCanvas();
		Global.LayerEditor.UpdateLayerList();
	}

	public void CreateNew(Vector2I size, BackgroundType backgroundType, bool reportToQuickInfo = true)
	{
		PreviousSavePath = null;
		HasUnsavedChanges = false;
		Create(size, backgroundType, null);

		if (reportToQuickInfo)
			Global.QuickInfo.Set("New canvas created!");
	}

	public void CreateFromData(Vector2I size, Layer[] layers) =>
		Create(size, null, layers);
	#endregion

	private void SetBackgroundTexture()
	{
		Global.BackgroundStyle.Texture = TextureGenerator.NewBackgroundTexture(Size * Global.Settings.Canvas.BGResolutionMult);

		//Disable texture filtering and set background node size
		BackgroundPanel.TextureFilter = TextureFilterEnum.Nearest;
		BackgroundPanel.Size = Size;
	}

	#region Layers
	public void NewLayer(BackgroundType backgroundType = BackgroundType.Transparent)
	{
		Layer layer = new(this, backgroundType);
		Layers.Insert(CurrentLayerIndex, layer);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MoveLayerUp(int index)
	{
		ulong selectedID = CurrentLayer.ID;

		int insertIndex = index - 1;
		if (insertIndex < 0)
			insertIndex = Layers.Count - 1;
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
		Layer layer = Layers[index];
		Layers.RemoveAt(index);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MergeDown(int index)
	{
		if (index == Layers.Count - 1)
			return;

		ulong selectedID = CurrentLayer.ID;

		Layer layer = Layers[index];
		Layers.RemoveAt(index);

		Layers[index].MergeUnder(layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);
		if (CurrentLayerIndex == -1)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerVisibility(ulong id, bool visible)
	{
		Layers.FirstOrDefault(l => l.ID == id).Visible = visible;
		UpdateEntireCanvas();
	}

	public void DuplicateLayer(int index)
	{
		ulong selectedID = CurrentLayer.ID;

		Layer layer = new(this, Layers[index]);
		Layers.Insert(index, layer);

		CurrentLayerIndex = GetLayerIndex(selectedID);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void DeleteLayer(int index)
	{
		if (Layers.Count == 1)
			return;

		Layers.RemoveAt(index);
		if (CurrentLayerIndex >= Layers.Count)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerOpacity(int index, float opacity)
	{
		Layers[index].Opacity = opacity;
		Layers[index].PreviewNeedsUpdate = true;
		UpdateEntireCanvas();
	}

	public int GetLayerIndex(ulong id) =>
		Layers.FindIndex(l => l.ID == id);

	public void SetLayerName(int index, string name) => Layers[index].Name = name;
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

		foreach (CanvasChunk chunk in Chunks)
		{
			if (!chunk.MarkedForUpdate && !UpdateAllChunks)
				continue;

			UpdateChunkMesh(chunk);
		}

		CurrentLayer.PreviewNeedsUpdate = true;
		UpdateAllChunks = false;
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

	private Image GetFlattenedImage()
	{
		FlattenLayers(Vector2I.Zero, Size);
		Image image = Image.CreateFromData(Size.X, Size.Y, false, Image.Format.Rgba8,
			FlattenedColors.ToByteArray());

		return image;
	}

	private byte[] SerializeToFormat(ImageFormat format) => format switch
	{
		ImageFormat.PNG => GetFlattenedImage().SavePngToBuffer(),
		ImageFormat.JPEG => GetFlattenedImage().SaveJpgToBuffer(),
		ImageFormat.WEBP => GetFlattenedImage().SaveWebpToBuffer(),
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

			Size = new(image.GetWidth(), image.GetHeight());
			layers = new Layer[] { new(this, image.GetColorsFromImage()) };
		}
		catch (Exception ex)
		{
			Main.ReportError("An error occurred while deserializing data (The file may be corrupt)", ex);

			CreateNew(new(DefaultResolution, DefaultResolution), BackgroundType.Transparent, false);
			return;
		}

		CreateFromData(Size, layers);
	}
	#endregion

	#region DataSavingAndLoading
	private void FileSelected(FileDialogType type, string file)
	{
		if (type == FileDialogType.Open)
			LoadDataFromFile(file);
		else
			SaveDataToFile(file);
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
				PreviousSavePath = file;
				DeserializeFromScrbl(data);
				break;
			case ImageFormat.PNG:
			case ImageFormat.JPEG:
			case ImageFormat.WEBP:
				PreviousSavePath = null;
				DeserializeFromFormat(data, format);
				break;
		}

		HasUnsavedChanges = false;
		Global.QuickInfo.Set($"File '{Path.GetFileName(file)}' loaded successfully!");
	}

	public void SaveDataToFile(string file)
	{
		if (File.Exists(file))
			File.Delete(file);

		string extension = Path.GetExtension(file);
		ImageFormat format = ImageFormatParser.FileExtensionToImageFormat(extension);
		if (format == ImageFormat.Invalid)
			throw new FileLoadException("Invalid file type", file);
		else if (format == ImageFormat.SCRIBBLE)
			PreviousSavePath = file;

		byte[] data = format switch
		{
			ImageFormat.SCRIBBLE => SerializeToScrbl(),
			_ => SerializeToFormat(format),
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
		if (string.IsNullOrEmpty(Global.Canvas.PreviousSavePath))
		{
			FileDialogs.Show(FileDialogType.Save);
			return false;
		}

		try
		{
			Global.Canvas.SaveDataToFile(Global.Canvas.PreviousSavePath);
		}
		catch (Exception ex)
		{
			Modal errorModal = Main.ReportError("An error occurred while saving file", ex);
			errorModal.Hidden += () => FileDialogs.Show(FileDialogType.Save);
			return false;
		}

		Global.InteractionBlocker.Hide();
		return true;
	}
	#endregion
}
