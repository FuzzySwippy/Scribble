namespace ColorBlender.Models.ColorModels.RGB;

/// <summary>
/// Represents a RGB color.
/// </summary>
public class RGB : IColorModel
{
	/// <summary>
	/// The Red channel as a double from 0 to 255.
	/// </summary>
	public double R { get; set; }

	/// <summary>
	/// The Green channel as a double from 0 to 255.
	/// </summary>
	public double G { get; set; }

	/// <summary>
	/// The Blue channel as a double from 0 to 255.
	/// </summary>
	public double B { get; set; }
	/// <summary>
	/// The Alpha channel as a fraction from 0 to 1.
	/// </summary>
	public double A { get; set; }

	/// <summary>
	/// Represents a RGB color.
	/// </summary>
	public RGB()
	{
	}

	/// <summary>
	/// Represents a RGB color.
	/// </summary>
	/// <param name="r">The <b>Red</b> channel with a range from "0" to "255".</param>
	/// <param name="g">The <b>Green</b> channel with a range from "0" to "255".</param>
	/// <param name="b">The <b>Blue</b> channel with a range from "0" to "255".</param>
	/// <param name="a">
	/// The <b>Alpha</b> channel with a range from "0" to "1".<br/>
	/// <i>Default value is "1"</i>.
	/// </param>
	public RGB(double r, double g, double b, double a = 1)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	protected bool Equals(RGB other) => R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);

	public override bool Equals(object obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((RGB)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = R.GetHashCode();
			hashCode = (hashCode * 397) ^ G.GetHashCode();
			hashCode = (hashCode * 397) ^ B.GetHashCode();
			hashCode = (hashCode * 397) ^ A.GetHashCode();
			return hashCode;
		}
	}
}
