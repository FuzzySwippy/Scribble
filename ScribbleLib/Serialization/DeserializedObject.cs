namespace Scribble.ScribbleLib.Serialization;

public class DeserializedObject
{
	public SerializationTypes Type { get; }
	public object Value { get; }

	public DeserializedObject(SerializationTypes type, object value)
	{
		Type = type;
		Value = value;
	}
}
