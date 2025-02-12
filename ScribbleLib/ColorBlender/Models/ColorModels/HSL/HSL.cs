namespace ColorBlender.Models.ColorModels.HSL;

/// <summary>
/// Represents a HSL color.
/// </summary>
public class HSL : IColorModel
{
	/// <summary>
	/// Hue channel as a double from 0 to 360.
	/// </summary>
	public double H { get; set; }

	/// <summary>
	/// Saturation channel as a double from 0 to 100.
	/// </summary>
	public double S { get; set; }

	/// <summary>
	/// Luminosity channel as a double from 0 to 100.
	/// </summary>
	public double L { get; set; }

	/// <summary>
	/// Alpha channel as a fraction from 0 to 1.
	/// </summary>
	public double A { get; set; }

	/// <summary>
	/// Represents a HSL color.
	/// </summary>
	public HSL()
	{
	}

	/// <summary>
	/// Represents a HSL color.
	/// </summary>
	/// <param name="h">The <b>Hue</b> value with a range from "0" to "360".</param>
	/// <param name="s">The <b>Saturation</b> value with a range from "0" to "100".</param>
	/// <param name="l">The <b>Luminosity</b> value with a range from "0" to "100".</param>
	/// <param name="a">
	/// The <b>Alpha</b> channel with a range from "0" to "1".<br/>
	/// <i>Default value is "1".</i>.
	/// </param>
	public HSL(double h, double s, double l, double a = 1)
	{
		H = h;
		S = s;
		L = l;
		A = a;
	}

	protected bool Equals(HSL other) => H == other.H && S == other.S && L == other.L && A == other.A;

	public override bool Equals(object obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((HSL)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = H.GetHashCode();
			hashCode = (hashCode * 397) ^ S.GetHashCode();
			hashCode = (hashCode * 397) ^ L.GetHashCode();
			hashCode = (hashCode * 397) ^ A.GetHashCode();
			return hashCode;
		}
	}
}
