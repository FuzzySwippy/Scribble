using System;
using System.Collections.Generic;
using Godot;

namespace Scribble.ScribbleLib.Formats;

public class BMP
{
	private const int FileHeaderSize = 54;

	public int Width { get; private set; }
	public int Height { get; private set; }
	public Image Image { get; private set; }

	public BMP(int width, int height)
	{
		Width = width;
		Height = height;
		Image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
	}

	public BMP(Image image)
	{
		Width = image.GetWidth();
		Height = image.GetHeight();
		Image = (Image)image.Duplicate(true);
	}

	public void SetPixel(int x, int y, Color color) =>
		Image.SetPixel(x, y, color);

	public Color GetPixel(int x, int y) =>
		Image.GetPixel(x, y);

	public byte[] Serialize()
	{
		List<byte> data = [];

		// File header (14 bytes)
		data.AddRange("BM"u8.ToArray()); // BM
		data.AddRange(BitConverter.GetBytes(Width * Height * 4 + FileHeaderSize)); // File size
		data.AddRange(new byte[4]); // Reserved
		data.AddRange(BitConverter.GetBytes(FileHeaderSize)); // Data offset

		// Info header (40 bytes)
		data.AddRange(BitConverter.GetBytes(40)); // Info header size
		data.AddRange(BitConverter.GetBytes(Width)); // Width
		data.AddRange(BitConverter.GetBytes(Height)); // Height
		data.AddRange(BitConverter.GetBytes((short)1)); // Planes
		data.AddRange(BitConverter.GetBytes((short)32)); // Bits per pixel
		data.AddRange(new byte[4]); // Compression (none)
		data.AddRange(new byte[4]); // Image size
		data.AddRange(new byte[4]); // X pixels per meter
		data.AddRange(new byte[4]); // Y pixels per meter
		data.AddRange(new byte[4]); // Colors used
		data.AddRange(new byte[4]); // Important colors (all)

		// Image data
		for (int y = Height - 1; y >= 0; y--)
		{
			for (int x = 0; x < Width; x++)
			{
				Color color = GetPixel(x, y);
				data.Add((byte)(color.B * 255));
				data.Add((byte)(color.G * 255));
				data.Add((byte)(color.R * 255));
				data.Add((byte)(color.A * 255));
			}
		}

		return data.ToArray();
	}

	/*public static BMP Deserialize(byte[] data)
	{
		//Check for valid BMP header
		if (data.Length > FileHeaderSize || data[0] != (byte)'B' || data[1] != (byte)'M')
			throw new Exception("Invalid BMP image header");

		int imageDataSize = BitConverter.ToInt32(data, 2) - FileHeaderSize;
		int width = BitConverter.ToInt32(data, 18);
		int height = BitConverter.ToInt32(data, 22);

		if (imageDataSize != width * height * 4)
			throw new Exception("Invalid BMP image data size");

		BMP bitmap = new BMP(width, height);
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				int index = y * width * 4 + x * 4 + FileHeaderSize;
				bitmap.SetPixel(x,y, Color.Color8(
					data[index + 2],
					data[index + 1],
					data[index],
					data[index + 3]));
			}
		}
		return bitmap;
	}*/
}
