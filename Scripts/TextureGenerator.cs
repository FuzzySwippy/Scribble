using Godot;
using ScribbleLib;

namespace Scribble;

public static class TextureGenerator
{
	public static Texture2D NewBackgroundTexture(Vector2I size)
	{
		//Generate the background image
		Image image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		if (Global.Settings.Canvas.BG_IsSolid)
			size.Loop((x, y) => image.SetPixel(x, y, Global.Settings.Canvas.BG_Primary));
		else
			size.Loop((x, y) => image.SetPixel(x, y, (x + y) % 2 == 0 ? Global.Settings.Canvas.BG_Primary : Global.Settings.Canvas.BG_Secondary));

		return ImageTexture.CreateFromImage(image);
	}
}
