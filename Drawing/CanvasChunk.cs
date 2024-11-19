using Godot;

namespace Scribble.Drawing;

public partial class CanvasChunk : TextureRect
{
	public Vector2I PixelPosition { get; private set; }
	public Vector2I SizeInPixels { get; private set; }

	private Image Image { get; set; }
	private ImageTexture ImageTexture { get; set; }

	public bool MarkedForUpdate { get; set; }

	public void Init(Vector2I position, Vector2I size)
	{
		Position = position;
		Size = size;
		PixelPosition = position;
		SizeInPixels = size;

		Generate();
		Show();
	}

	private void Generate()
	{
		Image = Image.CreateEmpty(SizeInPixels.X, SizeInPixels.Y, false, Image.Format.Rgba8);
		ImageTexture = ImageTexture.CreateFromImage(Image);

		Texture = ImageTexture;
	}

	public void SetColors(Color[,] colors)
	{
		for (int x = 0; x < SizeInPixels.X; x++)
			for (int y = 0; y < SizeInPixels.Y; y++)
				Image.SetPixel(x, y, colors[PixelPosition.X + x, PixelPosition.Y + y]);
	}

	public void Update()
	{
		ImageTexture.Update(Image);
		MarkedForUpdate = false;
	}

	public void Clear()
	{
		MarkedForUpdate = false;

		Image.Dispose();
		ImageTexture.Dispose();
		Hide();
	}
}
