using System;
using Scribble.ScribbleLib.Extensions;
using Image = Godot.Image;

namespace Scribble.ScribbleLib.Formats;

public class APNG(Image[] images, int frameTimeMs, bool loop, bool blackIsTransparent) : IDisposable
{
	private Image[] Images { get; set; } = images;
	private int FrameTimeMs { get; set; } = frameTimeMs;
	private bool Loop { get; set; } = loop;
	private bool BlackIsTransparent { get; set; } = blackIsTransparent;

	public byte[] Serialize(bool dispose)
	{
		Image baseImage = Images[0];
		Images = Images[1..];

		AnimatedImages.APNG apng = AnimatedImages.APNG.FromImage(baseImage.ToBitmap());

		foreach (Image image in Images)
			using (System.Drawing.Bitmap bitmap = image.ToBitmap())
				apng.AddFrame(bitmap);

		apng.PlayCount = Loop ? 0 : 1;
		apng.FrameTimeMs = FrameTimeMs < 11 ? 11 : FrameTimeMs;

		if (dispose)
			Dispose();
		return apng.Save();
	}

	public void Dispose()
	{
		foreach (Image image in Images)
			image.Dispose();
		Images = null;
	}
}
