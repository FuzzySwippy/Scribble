using Godot;
using GifMotion;
using Scribble.ScribbleLib.Extensions;
using System;
using System.IO;

namespace Scribble.ScribbleLib.Formats;

public class GIF(Image[] images, int frameTimeMs, bool loop, bool blackIsTransparent) : IDisposable
{
	private Image[] Images { get; set; } = images;
	private int FrameTimeMs { get; set; } = frameTimeMs;
	private bool Loop { get; set; } = loop;
	private bool BlackIsTransparent { get; set; } = blackIsTransparent;

	public byte[] Serialize()
	{
		MemoryStream memoryStream = new();

		using (TransparentGifCreator gifCreator = new(memoryStream, FrameTimeMs, Loop ? 0 : 1, BlackIsTransparent))
			foreach (Image image in Images)
				using (System.Drawing.Bitmap bitmap = image.ToBitmap())
					gifCreator.AddFrame(bitmap);

		return memoryStream.ToArray();
	}

	public void Dispose()
	{
		foreach (Image image in Images)
			image.Dispose();
		Images = null;
	}
}
