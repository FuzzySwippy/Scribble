using System;
using Godot;
using Newtonsoft.Json;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.ScribbleLib;

/// <summary>
/// A color class that doesn't update the Hue and Saturation values when the sum of RGB values is either 0 or 3.
/// Less performant alternative to the Godot Color class but a useful workaround for Hue and Saturation values being set to 0 when the RGB values are 0 or 3 (minimum or maximum Value(Lightness)).
/// </summary>
[Serializable]
public class ScribbleColor
{
	private float h, s, v;

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

	private float r, g, b;

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

	public ScribbleColor() { }

	public ScribbleColor(float r, float g, float b, float a = 1) => SetRGBA(r, g, b, a);

	public void SetRGB(float r, float g, float b) => SetRGBA(r, g, b, A);
	public void SetRGB255(int r, int g, int b) => SetRGBA((float)r / 255, (float)g / 255, (float)b / 255, A);

	public void SetHSV(float h, float s, float v) => SetHSVA(h, s, v, A);

	public void SetRGBA(float r, float g, float b, float a = 1)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		A = a;

		UpdateHSV();
	}

	public void SetHSVA(float h, float s, float v, float a = 1)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		A = a;

		UpdateRGB();
	}

	/// <summary>
	/// Sets color values from Godot HSV data
	/// </summary>
	/// <param name="color">Godot color</param>
	public void SetFromHSVA(Color color)
	{
		color.ToHsv(out float hue, out float saturation, out float value);

		if (saturation > 0 && saturation < 1 && value > 0 && value < 1)
			h = hue;

		if (value is > 0 and < 1)
			s = saturation;

		v = value;
		A = color.A;
		UpdateRGB();
	}

	/// <summary>
	/// Sets color values from Godot Color's RGBA data
	/// </summary>
	/// <param name="color">Godot color</param>
	public void Set(Color color)
	{
		r = color.R;
		g = color.G;
		b = color.B;
		A = color.A;
		UpdateHSV();
	}

	/// <summary>
	/// Sets color values from SimpleColor's RGBA data
	/// </summary>
	/// <param name="color">Simple color</param>
	public void Set(SimpleColor color)
	{
		r = color.R;
		g = color.G;
		b = color.B;
		A = color.A;
		UpdateHSV();
	}

	public void CloneFrom(ScribbleColor color)
	{
		r = color.R;
		g = color.G;
		b = color.B;

		h = color.H;
		s = color.S;
		v = color.V;

		A = color.A;
	}

	private void UpdateRGB()
	{
		Color color = Color.FromHsv(h, s, v);
		r = color.R;
		g = color.G;
		b = color.B;
	}

	private void UpdateHSV()
	{
		GodotColor.ToHsv(out float hue, out float saturation, out float value);

		if ((r + g + b).InRangeEx(0, 3))
		{
			h = hue;
			s = saturation;
		}
		v = value;
	}

	public uint ToRGBA32() => new Color(r, g, b, A).ToRgba32();


	//Overrides
	public bool Equals(ScribbleColor other) => r == other.r && g == other.g && b == other.b && A == other.A;

	public override bool Equals(object obj) => obj is ScribbleColor color && Equals(color);

	public static bool operator ==(ScribbleColor left, ScribbleColor right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ScribbleColor left, ScribbleColor right)
	{
		return !(left == right);
	}

	public override int GetHashCode() => (int)ToRGBA32();

	public override string ToString() => $"(RGB: {r}, {g}, {b} | HSV: {h}, {s}, {v} | A: {A})";
}
