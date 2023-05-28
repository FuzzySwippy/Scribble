using Newtonsoft.Json;

namespace ScribbleLib;

public partial class SimpleColor
{
	/// <summary>
	/// Red color component
	/// </summary>
	public float R { get; set; }

	/// <summary>
	/// Green color component
	/// </summary>
	public float G { get; set; }

	/// <summary>
	/// Blue color component
	/// </summary>
	public float B { get; set; }

	/// <summary>
	/// Alpha color component
	/// </summary>
	public float A { get; set; } = 1;

	/// <summary>
	/// Returns a new Godot Color with this SimpleColor's RGBA values
	/// </summary>
	[JsonIgnore]
	public Godot.Color GodotColor => new(R, G, B, A);

	/// <summary>
	/// Returns a new ScribbleColor with this SimpleColor's RGBA values
	/// </summary>
	[JsonIgnore]
	public ScribbleColor ScribbleColor => new(R, G, B, A);

	/// <summary>
	/// Gets the hex code of this color
	/// </summary>
	[JsonIgnore]
	public string HexCode => GodotColor.ToHtml();
}