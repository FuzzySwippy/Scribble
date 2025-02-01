using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/*
Original code taken from GifMotion source GifCreator class
All credits to the original author (warrengalyen)
*/

namespace GifMotion;

public class TransparentGifCreator : IDisposable
{
	private bool CreatedHeader { get; set; }
	private Stream Stream { get; }

	public string FilePath { get; }
	public int Delay { get; }
	public int Repeat { get; }
	public bool BlackIsTransparent { get; }
	public int FrameCount { get; private set; }

	public TransparentGifCreator(Stream stream, int delay = 33, int repeat = 0, bool blackIsTransparent = true)
	{
		Delay = delay;
		Repeat = repeat;
		BlackIsTransparent = blackIsTransparent;

		Stream = stream;
	}

	public TransparentGifCreator(string filePath, int delay = 33, int repeat = 0, bool transparentBackground = true)
	{
		FilePath = filePath;
		Delay = delay;
		Repeat = repeat;
		BlackIsTransparent = transparentBackground;

		Stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
	}

	public void Dispose() =>
		Finish();

	/// <summary>
	///     Add a new frame to the GIF
	/// </summary>
	/// <param name="image">The image to add to the GIF stack</param>
	/// <param name="delay">The delay in milliseconds this GIF will be delayed (-1: Indicating class property delay)</param>
	/// <param name="quality">The GIFs quality</param>
	public void AddFrame(Image image, int delay = -1, GIFQuality quality = GIFQuality.Default)
	{
		GifClass gif = new();
		gif.LoadGifImage(image, quality);

		if (!CreatedHeader)
		{
			AppendToStream(CreateHeaderBlock());
			AppendToStream(gif.ScreenDescriptor.ToArray());
			AppendToStream(CreateApplicationExtensionBlock(Repeat));
			CreatedHeader = true;
		}

		AppendToStream(CreateGraphicsControlExtensionBlock(delay > -1 ? delay : Delay));
		AppendToStream(gif.ImageDescriptor.ToArray());
		AppendToStream(gif.ColorTable.ToArray());
		AppendToStream(gif.ImageData.ToArray());

		FrameCount++;
	}

	/// <summary>
	///     Add a new frame to the GIF
	/// </summary>
	/// <param name="path">The image's path which will be added to the GIF stack</param>
	/// <param name="delay">The delay in milliseconds this GIF will be delayed (-1: Indicating class property delay)</param>
	/// <param name="quality">The GIFs quality</param>
	public void AddFrame(string path, int delay = -1, GIFQuality quality = GIFQuality.Default)
	{
		using Image img = Helper.LoadImage(path);
		AddFrame(img, delay, quality);
	}

	private void AppendToStream(byte[] data) =>
		Stream.Write(data, 0, data.Length);

	/// <summary>
	///     Add a new frame to the GIF async
	/// </summary>
	/// <param name="image">The image to add to the GIF stack</param>
	/// <param name="delay">The delay in milliseconds this GIF will be delayed (-1: Indicating class property delay)</param>
	/// <param name="quality">The GIFs quality</param>
	public async Task AddFrameAsync(Image image, int delay = -1, GIFQuality quality = GIFQuality.Default, CancellationToken cancellationToken = default)
	{
		GifClass gif = new();
		gif.LoadGifImage(image, quality);

		if (!CreatedHeader)
		{
			await AppendToStreamAsync(CreateHeaderBlock(), cancellationToken);
			await AppendToStreamAsync(gif.ScreenDescriptor.ToArray(), cancellationToken);
			await AppendToStreamAsync(CreateApplicationExtensionBlock(Repeat), cancellationToken);
			CreatedHeader = true;
		}

		await AppendToStreamAsync(CreateGraphicsControlExtensionBlock(delay > -1 ? delay : Delay), cancellationToken);
		await AppendToStreamAsync(gif.ImageDescriptor.ToArray(), cancellationToken);
		await AppendToStreamAsync(gif.ColorTable.ToArray(), cancellationToken);
		await AppendToStreamAsync(gif.ImageData.ToArray(), cancellationToken);

		FrameCount++;
	}

	/// <summary>
	///     Add a new frame to the GIF async
	/// </summary>
	/// <param name="path">The image's path which will be added to the GIF stack</param>
	/// <param name="delay">The delay in milliseconds this GIF will be delayed (-1: Indicating class property delay)</param>
	/// <param name="quality">The GIFs quality</param>
	public async Task AddFrameAsync(string path, int delay = -1, GIFQuality quality = GIFQuality.Default, CancellationToken cancellationToken = default)
	{
		using Image img = Helper.LoadImage(path);
		await AddFrameAsync(img, delay, quality, cancellationToken);
	}

	private Task AppendToStreamAsync(byte[] data, CancellationToken cancellationToken = default) =>
		Stream.WriteAsync(data, 0, data.Length, cancellationToken);

	/// <summary>
	///     Finish creating the GIF and start flushing
	/// </summary>
	private void Finish()
	{
		if (Stream == null)
			return;

		Stream.WriteByte(0x3B); // Image terminator
		if (Stream.GetType() == typeof(FileStream))
			Stream.Dispose();
	}

	/// <summary>
	///     Create the GIFs header block (GIF89a)
	/// </summary>
	private static byte[] CreateHeaderBlock() =>
		"GIF89a"u8.ToArray();

	private static byte[] CreateApplicationExtensionBlock(int repeat)
	{
		byte[] buffer =
		[
			0x21, // Extension introducer
			0xFF, // Application extension
			0x0B, // Size of block
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
			(byte)'0',
			0x03, // Size of block
			0x01, // Loop indicator
			(byte)(repeat % 0x100), // Number of repetitions
			(byte)(repeat / 0x100), // 0 for endless loop
			0x00, // Block terminator
		];
		return buffer;
	}

	private byte[] CreateGraphicsControlExtensionBlock(int delay)
	{
		byte[] buffer =
		[
			0x21, // Extension introducer
			0xF9, // Graphic control extension
			0x04, // Size of block
			(byte)(BlackIsTransparent ? 0x09 : 0x08), // Flags: reserved, disposal method, user input, transparent color
			(byte)(delay / 10 % 0x100), // Delay time low byte
			(byte)(delay / 10 / 0x100), // Delay time high byte
			0x00, // Transparent color index (Modified to use black as transparent color)
			0x00, // Block terminator
		];
		return buffer;
	}
}