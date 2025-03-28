﻿using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System;

namespace AnimatedImages;

/// <summary>
///     Describe a single frame.
/// </summary>
public class Frame
{
	/// <summary>
	/// The chunk signature.
	/// </summary>
	public static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

	private List<IDATChunk> idatChunks = [];
	private List<OtherChunk> otherChunks = [];

	/// <summary>
	///     Gets or Sets the acTL chunk
	/// </summary>
	internal IHDRChunk IHDRChunk { get; set; }

	/// <summary>
	///     Gets or Sets the fcTL chunk
	/// </summary>
	internal FcTLChunk FcTLChunk { get; set; }

	/// <summary>
	///     Gets or Sets the IEND chunk
	/// </summary>
	internal IENDChunk IENDChunk { get; set; }

	/// <summary>
	///     Gets or Sets the other chunks
	/// </summary>
	internal List<OtherChunk> OtherChunks
	{
		get => otherChunks;
		set => otherChunks = value;
	}

	/// <summary>
	///     Gets or Sets the IDAT chunks
	/// </summary>
	internal List<IDATChunk> IDATChunks
	{
		get => idatChunks;
		set => idatChunks = value;
	}

	/// <summary>
	///     Add an Chunk to end end of existing list.
	/// </summary>
	internal void AddOtherChunk(OtherChunk chunk) =>
		otherChunks.Add(chunk);

	/// <summary>
	///     Add an IDAT Chunk to end end of existing list.
	/// </summary>
	internal void AddIDATChunk(IDATChunk chunk) =>
		idatChunks.Add(chunk);

	/// <summary>
	///     Gets the frame as PNG FileStream.
	/// </summary>
	public MemoryStream GetStream()
	{
		IHDRChunk ihdrChunk = new(IHDRChunk);
		if (FcTLChunk != null)
		{
			// Fix frame size with fcTL data.
			ihdrChunk.ModifyChunkData(0, Helper.ConvertEndian(FcTLChunk.Width));
			ihdrChunk.ModifyChunkData(4, Helper.ConvertEndian(FcTLChunk.Height));
		}

		// Write image data
		MemoryStream ms = new();
		ms.WriteBytes(Signature);
		ms.WriteBytes(ihdrChunk.RawData);
		otherChunks.ForEach(o => ms.WriteBytes(o.RawData));
		idatChunks.ForEach(i => ms.WriteBytes(i.RawData));
		ms.WriteBytes(IENDChunk.RawData);

		ms.Position = 0;
		return ms;
	}

	/// <summary>
	/// Converts the Frame to a Bitmap
	/// </summary>
	/// <returns>The bitmap of the frame.</returns>
	public Bitmap ToBitmap()
	{
		// Create the bitmap
		Bitmap b = (Bitmap)Image.FromStream(GetStream());
		Bitmap final = new(IHDRChunk.Width, IHDRChunk.Height);

		Graphics g = Graphics.FromImage(final);
		g.CompositingMode = CompositingMode.SourceOver;
		g.CompositingQuality = CompositingQuality.GammaCorrected;
		g.Clear(Color.FromArgb(0x00000000));
		g.DrawImage(b, FcTLChunk.XOffset, FcTLChunk.YOffset, FcTLChunk.Width, FcTLChunk.Height);

		return final;
	}

	/// <summary>
	/// Gets or sets the frame rate.
	/// </summary>
	/// <value>The frame rate in milliseconds.</value>
	/// <remarks>Should not be less than 10 ms or animation will not occur.</remarks>
	public int FrameRate
	{
		get
		{
			int frameRate = FcTLChunk.DelayNumerator;
			double denominatorOffset = 1000 / FcTLChunk.DelayDenominator;
			if ((int)Math.Round(denominatorOffset) != 1) //If not millisecond based make it so for easier processing
				frameRate = (int)(FcTLChunk.DelayNumerator * denominatorOffset);

			return frameRate;
		}
		internal set
		{
			//Standardize to milliseconds.

			FcTLChunk.DelayNumerator = (ushort)(value);
			FcTLChunk.DelayDenominator = 1000;
		}
	}
}