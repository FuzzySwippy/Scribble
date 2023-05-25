namespace ScribbleLib;

/// <summary>
/// Simple color serializable class containing only RGBA values
/// </summary>
public partial class SimpleColor : IDuplicatable<SimpleColor>
{
	/// <summary>
	/// Creates a new SimpleColor with RGB values set to 0 and alpha set to 1
	/// </summary>
	public SimpleColor() { }


	/// <summary>
	/// Creates a new SimpleColor from the given Godot Color data
	/// </summary>
	public SimpleColor(Godot.Color color)
	{
		R = color.R;
		G = color.G;
		B = color.B;
		A = color.A;
	}

	/// <summary>
	/// Creates a new SimpleColor from the given ScribbleColor data
	/// </summary>
	public SimpleColor(ScribbleColor color)
	{
		R = color.R;
		G = color.G;
		B = color.B;
		A = color.A;
	}

	/// <summary>
	/// Creates a new SimpleColor with the given RGBA values
	/// </summary>
	/// <param name="r">Red color component</param>
	/// <param name="g">Green color component</param>
	/// <param name="b">Blue color component</param>
	/// <param name="a">Alpha color component</param>
	public SimpleColor(float r, float g, float b, float a = 1)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	/// <summary>
	/// Sets color RGBA values from Godot Color data
	/// </summary>
	public void Set(Godot.Color color)
	{
		R = color.R;
		G = color.G;
		B = color.B;
		A = color.A;
	}

	/// <summary>
	/// Sets color RGBA values from ScribbleColor data
	/// </summary>
	public void Set(ScribbleColor color)
	{
		R = color.R;
		G = color.G;
		B = color.B;
		A = color.A;
	}

	public SimpleColor Duplicate() => new(R, G, B, A);
}
