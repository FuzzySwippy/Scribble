using GifMotion;
using Scribble.ScribbleLib.Extensions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Imaging;

using Image = Godot.Image;

namespace Scribble.ScribbleLib.Formats;

public class GIF(Image[] images, int frameTimeMs, bool loop, bool blackIsTransparent) : IDisposable
{
	private static readonly byte[] Netscape20 = [
			(byte)'N', // NETSCAPE2.0
			(byte)'E',
			(byte)'T',
			(byte)'S',
			(byte)'C',
			(byte)'A',
			(byte)'P',
			(byte)'E',
			(byte)'2',
			(byte)'.',
			(byte)'0'
		];

	private Image[] Images { get; set; } = images;
	private int FrameTimeMs { get; set; } = frameTimeMs;
	private bool Loop { get; set; } = loop;
	private bool BlackIsTransparent { get; set; } = blackIsTransparent;

	public byte[] Serialize()
	{
		MemoryStream memoryStream = new();

		using (TransparentGifCreator gifCreator = new(memoryStream, FrameTimeMs, Loop, BlackIsTransparent))
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

	/// <summary>
	/// Loads a GIF from a byte buffer.
	/// </summary>
	/// <returns>Animation data if successful, otherwise null</returns>
	public static (List<Image> frames, bool loop, int frameTimeMs)? LoadFramesFromBuffer(byte[] data)
	{
		List<Image> frames = [];
		bool loop;
		int frameTimeMs;

		System.Drawing.Image gifImage = System.Drawing.Image.FromStream(new MemoryStream(data));
		if (gifImage.RawFormat.Equals(ImageFormat.Gif))
		{
			FrameDimension dimension = new(gifImage.FrameDimensionsList[0]);
			int frameCount = gifImage.GetFrameCount(dimension);

			loop = data[data.IndexOf(Netscape20) + Netscape20.Length + 2] == 0;

			PropertyItem frameTimeProperty = gifImage.GetPropertyItem(0x5100);
			frameTimeMs = BitConverter.ToUInt16(frameTimeProperty.Value, 0) * 10;

			for (int i = 0; i < frameCount; i++)
			{
				gifImage.SelectActiveFrame(dimension, i);

				using MemoryStream memoryStream = new();
				gifImage.Save(memoryStream, ImageFormat.Png);

				Image image = new();
				image.LoadPngFromBuffer(memoryStream.ToArray());
				frames.Add(image);
			}
		}
		else
		{
			gifImage.Dispose();
			return null;
		}

		return (frames, loop, frameTimeMs);
	}
}
