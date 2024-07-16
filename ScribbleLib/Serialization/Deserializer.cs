using System;
using System.Collections.Generic;
using Godot;

namespace Scribble.ScribbleLib.Serialization;

public class Deserializer
{
	public Dictionary<string, DeserializedObject> DeserializedObjects { get; } = new();

	public Deserializer(byte[] data) =>
		Deserialize(data);

	private void Deserialize(byte[] data)
	{
		int index = 0;
		while (index < data.Length)
		{
			int typeIndex = index;
			SerializationTypes type = (SerializationTypes)data[index++];

			index += ReadString(data, index, out string tag);

			object value = null;
			switch (type)
			{
				case SerializationTypes.Bool:
					index += ReadBool(data, index, out bool boolValue);
					value = boolValue;
					break;
				case SerializationTypes.Char:
					index += ReadChar(data, index, out char charValue);
					value = charValue;
					break;
				case SerializationTypes.Byte:
					value = data[index++];
					break;
				case SerializationTypes.SByte:
					value = (sbyte)data[index++];
					break;
				case SerializationTypes.Short:
					value = BitConverter.ToInt16(data, index);
					index += 2;
					break;
				case SerializationTypes.UShort:
					value = BitConverter.ToUInt16(data, index);
					index += 2;
					break;
				case SerializationTypes.Int:
					value = BitConverter.ToInt32(data, index);
					index += 4;
					break;
				case SerializationTypes.UInt:
					value = BitConverter.ToUInt32(data, index);
					index += 4;
					break;
				case SerializationTypes.Long:
					value = BitConverter.ToInt64(data, index);
					index += 8;
					break;
				case SerializationTypes.ULong:
					value = BitConverter.ToUInt64(data, index);
					index += 8;
					break;
				case SerializationTypes.Float:
					value = BitConverter.ToSingle(data, index);
					index += 4;
					break;
				case SerializationTypes.Double:
					value = BitConverter.ToDouble(data, index);
					index += 8;
					break;
				case SerializationTypes.String:
					index += ReadString(data, index, out string stringValue);
					value = stringValue;
					break;
				case SerializationTypes.ByteArray:
					index += ReadByteArray(data, index, out byte[] byteArrayValue);
					value = byteArrayValue;
					break;
				case SerializationTypes.DateTime:
					value = new DateTime(BitConverter.ToInt64(data, index));
					index += 8;
					break;
				case SerializationTypes.Vector2:
					index += ReadVector2(data, index, out Vector2 vector2Value);
					value = vector2Value;
					break;
				case SerializationTypes.Vector2I:
					index += ReadVector2I(data, index, out Vector2I vector2IValue);
					value = vector2IValue;
					break;
				case SerializationTypes.Color:
					index += ReadColor(data, index, out Color colorValue);
					value = colorValue;
					break;
				default:
					throw new Exception($"Unknown serialization type at index {typeIndex}");
			}

			DeserializedObjects.Add(tag, new DeserializedObject(type, value));
		}
	}

	public int ReadBool(byte[] data, int index, out bool value)
	{
		value = BitConverter.ToBoolean(data, index);
		return 1;
	}

	public int ReadChar(byte[] data, int index, out char value)
	{
		value = BitConverter.ToChar(data, index);
		return 2;
	}

	/// <summary>
	/// Reads a string from the data at the specified index.
	/// </summary>
	/// <returns>The string length plus 4 for the bytes used to store the string length</returns>
	private int ReadString(byte[] data, int index, out string value)
	{
		int length = BitConverter.ToInt32(data, index);
		value = System.Text.Encoding.UTF8.GetString(data, index + 4, length);
		return 4 + length;
	}

	private int ReadByteArray(byte[] data, int index, out byte[] value)
	{
		int length = BitConverter.ToInt32(data, index);
		value = new byte[length];
		Array.Copy(data, index + 4, value, 0, length);
		return 4 + length;
	}

	private int ReadVector2(byte[] data, int index, out Vector2 value)
	{
		value = new Vector2(BitConverter.ToSingle(data, index), BitConverter.ToSingle(data, index + 4));
		return 8;
	}

	private int ReadVector2I(byte[] data, int index, out Vector2I value)
	{
		value = new Vector2I(BitConverter.ToInt32(data, index), BitConverter.ToInt32(data, index + 4));
		return 8;
	}

	private int ReadColor(byte[] data, int index, out Color value)
	{
		value = new Color(BitConverter.ToUInt32(data, index));
		return 4;
	}
}
