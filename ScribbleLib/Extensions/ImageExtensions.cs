using System.Drawing;
using Bitmap = System.Drawing.Bitmap;
using Image = Godot.Image;

namespace Scribble.ScribbleLib.Extensions;

public static class ImageExtensions
{
	public static Bitmap ToBitmap(this Image image)
	{
		Bitmap bitmap = new(image.GetWidth(), image.GetHeight());
		for (int x = 0; x < image.GetWidth(); x++)
		{
			for (int y = 0; y < image.GetHeight(); y++)
			{
				Godot.Color imageColor = image.GetPixel(x, y);
				bitmap.SetPixel(x, y, Color.FromArgb(imageColor.A8, imageColor.R8, imageColor.G8, imageColor.B8));
			}
		}
		return bitmap;
	}
}
