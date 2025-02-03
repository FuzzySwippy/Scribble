using System.Drawing;

using Image = Godot.Image;

namespace Scribble.ScribbleLib.Extensions;

public static class BitmapExtensions
{
    public static Image ToImage(this Bitmap bitmap)
	{
		Image image = Image.CreateEmpty(bitmap.Width, bitmap.Height, false, Image.Format.Rgba8);
		for (int x = 0; x < bitmap.Width; x++)
		{
			for (int y = 0; y < bitmap.Height; y++)
			{
				System.Drawing.Color color = bitmap.GetPixel(x, y);
				image.SetPixel(x, y, Godot.Color.Color8(color.R, color.G, color.B, color.A));
			}
		}
		return image;
	}
}
