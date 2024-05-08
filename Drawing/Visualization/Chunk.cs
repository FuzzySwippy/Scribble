using System;
using Godot;

namespace Scribble.Drawing.Visualization;

public class Chunk : IDisposable
{
	public readonly Vector2I position;
	public readonly Vector2I size;

	private readonly Canvas canvas;
	private readonly CanvasLayer layer;

	private readonly TextureRect textureRect;

	private readonly ImageTexture texture;
	private readonly Image image;

	public Chunk(Vector2I size, Vector2I position, Canvas canvas, CanvasLayer layer)
	{
		this.canvas = canvas;
		this.layer = layer;
		this.size = size;
		this.position = position;

		image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		texture = ImageTexture.CreateFromImage(image);

		textureRect = new()
		{
			Name = $"Chunk[{position.X}, {position.Y}]",
			Position = position,
			Size = size,
			Texture = texture,
			StretchMode = TextureRect.StretchModeEnum.Keep,
			TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
		};

		layer.AddChild(textureRect);
	}

	/// <summary>
	/// Updates the chunk's texture with the canvas data.
	/// </summary>
	public void Update()
	{
		layer.DirtyPixels.IterateRange(position, position + size, UpdatePixel);
		texture.Update(image);
	}

	private void UpdatePixel(Vector2I pos) =>
		image.SetPixel(pos.X - position.X, pos.Y - position.Y, canvas.GetImagePixel(pos));

	public void Dispose()
	{
		textureRect.QueueFree();
		textureRect.Dispose();

		texture.Dispose();

		image.Free();
		image.Dispose();
	}
}
