using System;
using Godot;
using Color = Godot.Color;

namespace Scribble.Drawing.Visualization;

public class Chunk : IDisposable
{
	public readonly Vector2I position;
	public readonly Vector2I size;

	readonly Canvas canvas;

	readonly Control parent;
	readonly TextureRect textureRect;
	readonly TextureRect backgroundTextureRect;
	readonly ImageTexture texture;
	readonly Image image;

	public Chunk(Vector2I size, Vector2I position, Canvas canvas, ImageCanvas imageCanvas, Texture2D backgroundTexture)
	{
		this.canvas = canvas;
		this.size = size;
		this.position = position;

		image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		texture = ImageTexture.CreateFromImage(image);

		textureRect = new()
		{
			Texture = texture,
			StretchMode = TextureRect.StretchModeEnum.Keep,
			Position = Vector2.Zero,
			Size = size,
		};

		backgroundTextureRect = new()
		{
			Texture = backgroundTexture,
			StretchMode = TextureRect.StretchModeEnum.Keep,
			TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
			Position = Vector2.Zero,
			Size = size
		};

		parent = new()
		{
			Name = $"Chunk[{position.X}, {position.Y}]",
			Size = size,
			Position = position
		};
		imageCanvas.AddChild(parent);
		parent.AddChild(backgroundTextureRect);
		parent.AddChild(textureRect);
	}

	public void Update()
	{
		Color? color;
		for (int x = position.X; x < position.X + size.X; x++)
		{
			for (int y = position.Y; y < position.Y + size.Y; y++)
			{
				color = canvas.RecalculatePixel(x, y);
				if (!color.HasValue)
					continue;

				image.SetPixel(x - position.X, y - position.Y, color.Value);
			}
		}
		texture.Update(image);
	}

	public Color GetPixel(Vector2I position) => image.GetPixelv(position);

	public Color GetPixel(int x, int y) => image.GetPixel(x, y);


	public void Dispose()
	{
		textureRect.QueueFree();
		textureRect.Dispose();

		backgroundTextureRect.QueueFree();
		backgroundTextureRect.Dispose();

		parent.QueueFree();
		parent.Dispose();

		texture.Free();
		texture.Dispose();

		image.Free();
		image.Dispose();
	}
}
