namespace Scribble.ScribbleLib.Serialization;

public enum SerializationTypes : byte
{
	Bool = 0,
	Char = 1,

	Byte = 2,
	SByte = 3,
	Short = 4,
	UShort = 5,
	Int = 6,
	UInt = 7,
	Long = 8,
	ULong = 9,

	Float = 10,
	Double = 11,

	String = 12,
	ByteArray = 13,

	DateTime = 14,

	Vector2 = 15,
	Vector2I = 16,

	Color = 17,
}
