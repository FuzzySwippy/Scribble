using Godot;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Application;

public static class TextureGenerator
{
	public static Texture2D NewBackgroundTexture(Vector2I size)
	{
		//Generate the background image
		Image image = Image.Create(size.X, size.Y, false, Image.Format.Rgba8);
		if (Global.Settings.Canvas.BGIsSolid)
			size.Loop((x, y) => image.SetPixel(x, y, Global.Settings.Canvas.BGPrimary));
		else
			size.Loop((x, y) => image.SetPixel(x, y, (x + y) % 2 == 0 ? Global.Settings.Canvas.BGPrimary : Global.Settings.Canvas.BGSecondary));

		return ImageTexture.CreateFromImage(image);
	}
}
