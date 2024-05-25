using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Scribble.ScribbleLib.Serialization;

namespace Scribble.ScribbleLib;

public class Serializer
{
	private bool Finished { get; set; }
	private List<byte> Data { get; } = new();
	private HashSet<string> Tags { get; } = new();

	public byte[] FinalData { get; private set; }

	public byte[] Finish()
	{
		Finished = true;
		FinalData = Data.ToArray();
		return FinalData;
	}

	public void Write<T>(T item, string tag)
	{
		if (Finished)
			throw new Exception("Serializer has already been finished");

		if (Tags.Contains(tag))
			throw new Exception("Tag already exists");

		switch (typeof(T))
		{
			case Type _ when typeof(T) == typeof(bool):
				AddData(SerializationTypes.Bool, tag, BitConverter.GetBytes((bool)(object)item));
				break;
			case Type _ when typeof(T) == typeof(char):
				AddData(SerializationTypes.Char, tag, BitConverter.GetBytes((char)(object)item));
				break;
			case Type _ when typeof(T) == typeof(byte):
				AddData(SerializationTypes.Byte, tag, new byte[] { (byte)(object)item });
				break;
			case Type _ when typeof(T) == typeof(sbyte):
				AddData(SerializationTypes.SByte, tag, new byte[] { (byte)(object)item });
				break;
			case Type _ when typeof(T) == typeof(short):
				AddData(SerializationTypes.Short, tag, BitConverter.GetBytes((short)(object)item));
				break;
			case Type _ when typeof(T) == typeof(ushort):
				AddData(SerializationTypes.UShort, tag, BitConverter.GetBytes((ushort)(object)item));
				break;
			case Type _ when typeof(T) == typeof(int):
				AddData(SerializationTypes.Int, tag, BitConverter.GetBytes((int)(object)item));
				break;
			case Type _ when typeof(T) == typeof(uint):
				AddData(SerializationTypes.UInt, tag, BitConverter.GetBytes((uint)(object)item));
				break;
			case Type _ when typeof(T) == typeof(long):
				AddData(SerializationTypes.Long, tag, BitConverter.GetBytes((long)(object)item));
				break;
			case Type _ when typeof(T) == typeof(ulong):
				AddData(SerializationTypes.ULong, tag, BitConverter.GetBytes((ulong)(object)item));
				break;
			case Type _ when typeof(T) == typeof(float):
				AddData(SerializationTypes.Float, tag, BitConverter.GetBytes((float)(object)item));
				break;
			case Type _ when typeof(T) == typeof(double):
				AddData(SerializationTypes.Double, tag, BitConverter.GetBytes((double)(object)item));
				break;
			case Type _ when typeof(T) == typeof(string):
				AddData(SerializationTypes.String, tag, SerializeString((string)(object)item));
				break;
			case Type _ when typeof(T) == typeof(byte[]):
				AddData(SerializationTypes.ByteArray, tag, SerializeByteArray((byte[])(object)item));
				break;
			case Type _ when typeof(T) == typeof(DateTime):
				AddData(SerializationTypes.DateTime, tag, BitConverter.GetBytes(((DateTime)(object)item).Ticks));
				break;
			case Type _ when typeof(T) == typeof(Vector2):
				AddData(SerializationTypes.Vector2, tag, SerializeVector2((Vector2)(object)item));
				break;
			case Type _ when typeof(T) == typeof(Vector2I):
				AddData(SerializationTypes.Vector2I, tag, SerializeVector2I((Vector2I)(object)item));
				break;
			default:
				throw new Exception("Unsupported type");
		}
	}

	private void AddData(SerializationTypes type, string tag, byte[] data)
	{
		Data.Add((byte)type);
		Data.AddRange(SerializeString(tag));
		Data.AddRange(data);
	}

	private byte[] SerializeString(string str)
	{
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
		byte[] length = BitConverter.GetBytes(bytes.Length);
		return length.Concat(bytes).ToArray();
	}

	private byte[] SerializeByteArray(byte[] bytes)
	{
		byte[] length = BitConverter.GetBytes(bytes.Length);
		return length.Concat(bytes).ToArray();
	}

	private byte[] SerializeVector2(Vector2 vector)
	{
		byte[] x = BitConverter.GetBytes(vector.X);
		byte[] y = BitConverter.GetBytes(vector.Y);
		return x.Concat(y).ToArray();
	}

	private byte[] SerializeVector2I(Vector2I vector)
	{
		byte[] x = BitConverter.GetBytes(vector.X);
		byte[] y = BitConverter.GetBytes(vector.Y);
		return x.Concat(y).ToArray();
	}
}
