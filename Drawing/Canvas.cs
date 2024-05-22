using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.Application;
using Scribble.ScribbleLib;
using Scribble.ScribbleLib.Extensions;
using Scribble.ScribbleLib.Input;
using Scribble.UI;

namespace Scribble.Drawing;
public partial class Canvas : Node2D
{
	public const int ChunkSize = 32;
	private Vector2I StandardChunkSize { get; } = new(ChunkSize, ChunkSize);
	private float BaseScale { get; } = 2048;

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
			GD.Print($"V: {CurrentLayer.Visible}");
		}
	}
	public Layer CurrentLayer => Layers[CurrentLayerIndex];
	private DateTime LayerPreviewLastUpdate { get; set; } = DateTime.Now;
	private TimeSpan LayerUpdateInterval { get; } = TimeSpan.FromMilliseconds(250);

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

		CreateNew(size);
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
	}

	public Color GetPixel(Vector2I position) =>
		position.X < 0 || position.Y < 0 || position.X >= Size.X ||
		position.Y >= Size.Y ? new() : CurrentLayer.GetPixel(position);

	//New
	private void CreateNew(Vector2I size)
	{
		Size = size;
		UpdateScale();
		SetBackgroundTexture();

		Layers.Clear();
		NewLayer();

		FlattenedColors = new Color[Size.X, Size.Y];
		GenerateChunks();

		//Position the camera's starting position in the middle of the canvas
		Global.Camera.Position = SizeInWorld / 2;
	}

	private void SetBackgroundTexture()
	{
		Global.BackgroundStyle.Texture = TextureGenerator.NewBackgroundTexture(Size * Global.Settings.Canvas.BGResolutionMult);

		//Disable texture filtering and set background node size
		BackgroundPanel.TextureFilter = TextureFilterEnum.Nearest;
		BackgroundPanel.Size = Size;
	}

	#region Layers
	public void NewLayer()
	{
		Layer layer = new(this);
		Layers.Insert(CurrentLayerIndex, layer);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MoveLayerUp()
	{
		int insertIndex = CurrentLayerIndex - 1;
		if (insertIndex < 0)
			insertIndex = Layers.Count - 1;
		Layer layer = CurrentLayer;
		Layers.RemoveAt(CurrentLayerIndex);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = insertIndex;
		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MoveLayerDown()
	{
		int insertIndex = CurrentLayerIndex + 1;
		if (insertIndex >= Layers.Count)
			insertIndex = 0;
		Layer layer = CurrentLayer;
		Layers.RemoveAt(CurrentLayerIndex);
		Layers.Insert(insertIndex, layer);

		CurrentLayerIndex = insertIndex;
		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void MergeDown()
	{
		if (CurrentLayerIndex == Layers.Count - 1)
			return;

		Layer layer = CurrentLayer;
		Layers.RemoveAt(CurrentLayerIndex);

		Layers[CurrentLayerIndex].MergeUnder(layer);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerVisibility(ulong id, bool visible)
	{
		Layers.FirstOrDefault(l => l.ID == id).Visible = visible;
		UpdateEntireCanvas();
	}

	public void DuplicateLayer()
	{
		Layer layer = new(this, CurrentLayer);
		Layers.Insert(CurrentLayerIndex, layer);

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void DeleteLayer()
	{
		if (Layers.Count == 1)
			return;

		Layers.RemoveAt(CurrentLayerIndex);
		if (CurrentLayerIndex >= Layers.Count)
			CurrentLayerIndex = Layers.Count - 1;

		Global.LayerEditor.UpdateLayerList();
		UpdateEntireCanvas();
	}

	public void SetLayerOpacity(float opacity)
	{
		CurrentLayer.Opacity = opacity;
		CurrentLayer.PreviewNeedsUpdate = true;
		UpdateEntireCanvas();
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
}