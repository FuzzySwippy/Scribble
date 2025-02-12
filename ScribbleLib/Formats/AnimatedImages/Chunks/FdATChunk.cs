using System;
using System.IO;

namespace AnimatedImages;

/// <summary>
/// Animated PNG FDAT Chunk
/// </summary>
internal class FdATChunk : Chunk
{
	private uint sequenceNumber;
	private byte[] frameData;

	/// <summary>
	/// Initializes a new instance of the <see cref="FdATChunk"/> class.
	/// </summary>
	/// <param name="bytes">Byte array of chunk data.</param>
	public FdATChunk(byte[] bytes) : base(bytes) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="FdATChunk"/> class.
	/// </summary>
	/// <param name="ms">Memory stream of chunk data.</param>
	public FdATChunk(MemoryStream ms) : base(ms) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="FdATChunk"/> class.
	/// </summary>
	/// <param name="chunk">Chunk data.</param>
	public FdATChunk(Chunk chunk) : base(chunk) { }

	/// <summary>
	/// Gets or sets the frame sequence number.
	/// </summary>
	/// <value>The sequence number.</value>
	public uint SequenceNumber
	{
		get => sequenceNumber;
		internal set
		{
			sequenceNumber = value;
			ModifyChunkData(0, Helper.ConvertEndian(value));
		}
	}

	/// <summary>
	/// Gets or sets the frame data.
	/// </summary>
	/// <value>The frame data.</value>
	public byte[] FrameData
	{
		get => frameData;
		internal set
		{
			frameData = value;
			ModifyChunkData(4, value);
		}
	}

	/// <summary>
	/// Parses the data.
	/// </summary>
	/// <param name="ms">Memory Stream of chunk data.</param>
	protected override void ParseData(MemoryStream ms)
	{
		sequenceNumber = Helper.ConvertEndian(ms.ReadUInt32());
		frameData = ms.ReadBytes((int)Length - 4);
	}

	/// <summary>
	/// Converts an FDAT Chunk to an IDAT Chunk.
	/// </summary>
	/// <returns>The IDAT chunk.</returns>
	public IDATChunk ToIDATChunk()
	{
		uint newCrc;
		using (MemoryStream msCrc = new())
		{
			msCrc.WriteBytes("IDAT"u8.ToArray());
			msCrc.WriteBytes(FrameData);

			newCrc = CrcHelper.Calculate(msCrc.ToArray());
		}

		using MemoryStream ms = new();
		ms.WriteUInt32(Helper.ConvertEndian(Length - 4));
		ms.WriteBytes("IDAT"u8.ToArray());
		ms.WriteBytes(FrameData);
		ms.WriteUInt32(Helper.ConvertEndian(newCrc));
		ms.Position = 0;

		return new IDATChunk(ms);
	}

	/// <summary>
	/// Creates an FDAT Chunk from an IDAT Chunk.
	/// </summary>
	/// <returns>The FDAT chunk.</returns>
	/// <param name="idatChunk">IDAT chunk.</param>
	/// <param name="sequenceNumber">Sequence number.</param>
	public static FdATChunk FromIDATChunk(IDATChunk idatChunk, uint sequenceNumber)
	{
		uint newCrc;
		byte[] frameData;
		FdATChunk fdat = null;

		using (MemoryStream msCrc = new())
		{
			msCrc.WriteBytes("fdAT"u8.ToArray());
			byte[] seqNum = BitConverter.GetBytes(Helper.ConvertEndian(sequenceNumber));
			frameData = new byte[seqNum.Length + idatChunk.ChunkData.Length];
			Buffer.BlockCopy(seqNum, 0, frameData, 0, seqNum.Length);
			Buffer.BlockCopy(idatChunk.ChunkData, 0, frameData, seqNum.Length, idatChunk.ChunkData.Length);
			msCrc.WriteBytes(frameData);

			newCrc = CrcHelper.Calculate(msCrc.ToArray());
		}

		using (MemoryStream ms = new())
		{
			ms.WriteUInt32((uint)Helper.ConvertEndian(idatChunk.ChunkData.Length + 4));
			ms.WriteBytes("fdAT"u8.ToArray());
			ms.WriteBytes(frameData);
			ms.WriteUInt32(Helper.ConvertEndian(newCrc));
			ms.Position = 0;

			fdat = new FdATChunk(ms);
		}

		return fdat;
	}
}