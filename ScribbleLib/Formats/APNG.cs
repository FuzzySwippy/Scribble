using System;
using System.Collections.Generic;
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
		using System.Drawing.Bitmap baseBitmap = Images[0].ToBitmap();
		AnimatedImages.APNG apng = AnimatedImages.APNG.FromImage(baseBitmap);

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

	/// <summary>
	/// Loads an APNG from a byte buffer.
	/// </summary>
	/// <returns>Animation data if successful, otherwise null</returns>
	public static (List<Image> frames, bool loop, int frameTimeMs)? LoadFramesFromBuffer(byte[] data)
	{
		List<Image> frames = [];
		bool loop;
		int frameTimeMs;

		try
		{
			AnimatedImages.APNG apng = AnimatedImages.APNG.FromStream(new(data));
			loop = apng.PlayCount == 0;
			frameTimeMs = apng.FrameTimeMs;

			foreach (AnimatedImages.Frame frame in apng.Frames)
				using (System.Drawing.Bitmap bitmap = frame.ToBitmap())
					frames.Add(bitmap.ToImage());
		}
		catch (System.Exception)
		{
			return null;
		}

		return (frames, loop, frameTimeMs);
	}
}
