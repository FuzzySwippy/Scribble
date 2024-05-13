using System;
using Godot;
using Newtonsoft.Json;

namespace ScribbleLib;

/// <summary>
/// A color class that doesn't update the Hue and Saturation values when the sum of RGB values is either 0 or 3.
/// Less performant alternative to the Godot Color class but a useful workaround for Hue and Saturation values being set to 0 when the RGB values are 0 or 3 (minimum or maximum Value(Lightness)).
/// </summary>
[Serializable]
public partial class ScribbleColor
{
	float h, s, v;

	/// <summary>
	/// Hue color component
	/// </summary>
	public float H
	{
		get => h;
		set
		{
			h = value;
			UpdateRGB();
		}
	}

	/// <summary>
	/// Saturation color component
	/// </summary>
	public float S
	{
		get => s;
		set
		{
			s = value;
			UpdateRGB();
		}
	}

	/// <summary>
	/// Value(Lightness) color component
	/// </summary>
	public float V
	{
		get => v;
		set
		{
			v = value;
			UpdateRGB();
		}
	}


	float r, g, b;

	/// <summary>
	/// Red color component
	/// </summary>
	public float R
	{
		get => r;
		set
		{
			r = value;
			UpdateHSV();
		}
	}

	/// <summary>
	/// Green color component
	/// </summary>
	public float G
	{
		get => g;
		set
		{
			g = value;
			UpdateHSV();
		}
	}

	/// <summary>
	/// Blue color component
	/// </summary>
	public float B
	{
		get => b;
		set
		{
			b = value;
			UpdateHSV();
		}
	}

	/// <summary>
	/// Alpha color component. Defaults to 1.
	/// </summary>
	public float A { get; set; } = 1;


	[JsonIgnore]
	public Color GodotColor => new(r, g, b, A);

	[JsonIgnore]
	public Color GodotColorOpaque => new(r, g, b, 1);

	[JsonIgnore]
	public SimpleColor SimpleColor => new(r, g, b, A);
}
