using System;
using ColorBlender.Models.ColorModels.HEX.Enums;

namespace ColorBlender.Models.ColorModels.HEX;

/// <summary>
/// Represents a HEX color.
/// </summary>
public class HEX : IColorModel
{
	/// <summary>
	/// The Red channel as a hexadecimal string from "00" to "ff".
	/// </summary>
	public string R { get; set; }

	/// <summary>
	/// The Green channel as a hexadecimal string from "00" to "ff".
	/// </summary>
	public string G { get; set; }

	/// <summary>
	/// The Blue channel as a hexadecimal string from "00" to "ff".
	/// </summary>
	public string B { get; set; }

	/// <summary>
	/// The Alpha channel as a hexadecimal string from "00" to "ff".
	/// </summary>
	public string A { get; set; }

	public HEX()
	{
	}

	/// <summary>
	/// Represents a HEX color.
	/// </summary>
	/// <param name="r">The <b>Red</b> channel with a range from "00" to "ff".</param>
	/// <param name="g">The <b>Green</b> channel with a range from "00" to "ff".</param>
	/// <param name="b">The <b>Blue</b> channel with a range from "00" to "ff".</param>
	/// <param name="a">
	/// The <b>Alpha</b> channel with a range from "00" to "ff".<br/>
	/// <i>Default value is "ff".</i>.
	/// </param>
	public HEX(string r, string g, string b, string a = "ff")
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	/// <summary>
	/// Represents a HEX color.
	/// </summary>
	/// <param name="color">Color in the form of a hexadecimal string. It can be in the form of the following types:<br/>
	/// <c>#RGB</c>, <c>#RRGGBB</c>, <c>#AARRGGBB</c>, <c>#RRGGBBAA</c>.
	/// </param>
	/// <param name="type">
	/// Hex color type with <b>leading</b> or <b>trailing</b> Alpha channel position.<br/>
	/// <i>Default value is "<see cref="EHEXFormat.HEXA"/>"</i>.
	/// </param>
	/// <remarks>Leading "#" is optional.</remarks>
	public HEX(string color, EHEXFormat type = EHEXFormat.HEXA)
	{
		HEX parsedColor = ParseString(color, type);
		R = parsedColor.R;
		G = parsedColor.G;
		B = parsedColor.B;
		A = parsedColor.A;
	}

	/// <summary>
	/// Hexadecimal string parser.
	/// </summary>
	/// <param name="color">Hexadecimal color in a form of a string.</param>
	/// <param name="type">Alpha channel position (<b>leading</b> or <b>trailing</b>).</param>
	/// <returns>Populated Hex object.</returns>
	private HEX ParseString(string color, EHEXFormat type = EHEXFormat.HEXA)
	{
		// Remove leading '#' if present
		if (color.StartsWith('#'))
			color = color[1..];

		// Short format
		if (color.Length == 3)
		{
			string r = color[..1];
			string g = color.Substring(1, 1);
			string b = color.Substring(2, 1);

			return new HEX(r + r, g + g, b + b, "ff");
		}

		// Without alpha
		if (color.Length == 6)
			return new HEX(color[..2], color.Substring(2, 2), color.Substring(4, 2), "ff");

		// Trailing alpha 
		if (type == EHEXFormat.HEXA)
			return new HEX(color[..2], color.Substring(2, 2), color.Substring(4, 2), color.Substring(6, 2));

		// Leading alpha
		return new HEX(color.Substring(2, 2), color.Substring(4, 2), color.Substring(6, 2), color[..2]);
	}

	/// <summary>
	/// Converts an object into a string representation with additional options.
	/// </summary>
	/// <param name="outputFormat">Alpha channel position and visibility.</param>
	/// <param name="hashSignVisibility">HashSign visibility.</param>
	/// <returns>An object in the form of a string.</returns>
	public string ToString(EHEXOutputFormat outputFormat = EHEXOutputFormat.HEXAOpt, EHashSignFormat hashSignVisibility = EHashSignFormat.Visible)
	{
		string result = string.Empty;
		string rgb = R + G + B;

		if (hashSignVisibility == EHashSignFormat.Visible)
			result += "#";

		return outputFormat switch
		{
			EHEXOutputFormat.HEXAOpt when string.Equals(A, "ff", StringComparison.OrdinalIgnoreCase) => result + rgb,
			EHEXOutputFormat.HEXAOpt => result + rgb + A,
			EHEXOutputFormat.OptAHEX when string.Equals(A, "ff", StringComparison.OrdinalIgnoreCase) => result + rgb,
			EHEXOutputFormat.OptAHEX => result + A + rgb,
			EHEXOutputFormat.ConstAHEX => result + A + rgb,
			EHEXOutputFormat.HEXAConst => result + rgb + A,
			_ => result,
		};
	}

	protected bool Equals(HEX other) => R == other.R && G == other.G && B == other.B && A == other.A;

	public override bool Equals(object obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((HEX)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = (R != null ? R.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (G != null ? G.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (B != null ? B.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (A != null ? A.GetHashCode() : 0);
			return hashCode;
		}
	}
}
