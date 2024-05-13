using Godot;
using ScribbleLib;
using Scribble.Application;

namespace Scribble.Drawing.Visualization;

public partial class Background : Control
{
	private Canvas canvas;
	private Vector2I EndSizes => canvas.EndChunkSizes;
	private Vector2I Length => canvas.ChunkRange;

	private Texture2D backgroundTexture;
	private Texture2D xEndBackgroundTexture;
	private Texture2D yEndBackgroundTexture;
	private Texture2D xYEndBackgroundTexture;


	private TextureRect[,] textureRects;

	public override void _Ready() =>
		backgroundTexture = GetBackgroundTexture(new(Canvas.PixelsPerChunk, Canvas.PixelsPerChunk));

	private void GenerateBackgroundTextures()
	{
		if (EndSizes.X > 0)
			xEndBackgroundTexture = GetBackgroundTexture(new(EndSizes.X, Canvas.PixelsPerChunk));

		if (EndSizes.Y > 0)
			yEndBackgroundTexture = GetBackgroundTexture(new(Canvas.PixelsPerChunk, EndSizes.Y));

		if (EndSizes.X > 0 && EndSizes.Y > 0)
			xYEndBackgroundTexture = GetBackgroundTexture(EndSizes);
	}

	private Texture2D GetBackgroundTexture(Vector2I size) =>
		TextureGenerator.NewBackgroundTexture(size * Global.Settings.Canvas.BG_ResolutionMult);

	public void Initialize()
	{
		canvas = Global.Canvas;
		GenerateBackgroundTextures();
		GenerateBackground();
	}

	private void GenerateBackground()
	{
		textureRects = new TextureRect[Length.X, Length.Y];

		Vector2I size;
		Texture2D texture;

		for (int x = 0; x < Length.X; x++)
		{
			for (int y = 0; y < Length.Y; y++)
			{
				texture = backgroundTexture;
				size = Canvas.DefaultChunkSize;

				//Get chunk size and texture
				if (x == Length.X - 1 && EndSizes.X > 0)
				{
					size.X = EndSizes.X;
					texture = xEndBackgroundTexture;
				}

				if (y == Length.Y - 1 && EndSizes.Y > 0)
				{
					size.Y = EndSizes.Y;
					texture = yEndBackgroundTexture;
					if (x == Length.X - 1 && EndSizes.X > 0)
						texture = xYEndBackgroundTexture;
				}

				//Create texture rect
				textureRects[x, y] = new()
				{
					Name = $"Chunk[{x}, {y}]",
					Position = new Vector2I(x, y) * Canvas.PixelsPerChunk,
					Size = size,
					Texture = texture,
					StretchMode = TextureRect.StretchModeEnum.Keep,
					TextureFilter = TextureFilterEnum.Nearest
				};

				AddChild(textureRects[x, y]);
			}
		}
	}

	public void Cleanup()
	{
		xEndBackgroundTexture?.Dispose();
		yEndBackgroundTexture?.Dispose();
		xYEndBackgroundTexture?.Dispose();

		For.Loop2(Length.X, Length.Y, (x, y) => textureRects[x, y].Dispose());
		textureRects = null;

		canvas = null;
	}
}
