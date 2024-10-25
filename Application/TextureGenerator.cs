using Godot;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Application;

public static class TextureGenerator
{
	/// <summary>
	/// Background primary color
	/// </summary>
	private static Color BGPrimary => ColorTools.GrayscaleColor(0.6f);

	/// <summary>
	/// Background secondary color
	/// </summary>
	private static Color BGSecondary => ColorTools.GrayscaleColor(0.4f);

	public static Texture2D NewBackgroundTexture(Vector2I size)
	{
		//Generate the background image
		Image image = Image.CreateEmpty(size.X, size.Y, false, Image.Format.Rgba8);
		size.Loop((x, y) =>
			image.SetPixel(x, y, (x + y) % 2 == 0 ?
				BGPrimary : BGSecondary));

		//Create the texture
		Texture2D texture = ImageTexture.CreateFromImage(image);
		image.Dispose();
		return texture;
	}
}
