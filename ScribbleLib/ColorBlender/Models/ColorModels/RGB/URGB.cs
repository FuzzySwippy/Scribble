namespace ColorBlender.Models.ColorModels.RGB;

/// <summary>
/// Represents a URGB color.
/// </summary>
public class URGB : IColorModel
{
	/// <summary>
	/// Red channel as a fraction from 0 to 1
	/// </summary>
	public decimal R { get; set; }

	/// <summary>
	/// Green channel as a fraction from 0 to 1
	/// </summary>
	public decimal G { get; set; }

	/// <summary>
	/// Blue channel as a fraction from 0 to 1
	/// </summary>
	public decimal B { get; set; }

	/// <summary>
	/// Alpha channel as a fraction from 0 to 1
	/// </summary>
	public decimal A { get; set; }

	/// <summary>
	/// Represents a URGB color.
	/// </summary>
	public URGB() { }

	/// <summary>
	/// Represents a RGB color.
	/// </summary>
	/// <param name="r">The <b>Red</b> channel with a range from "0" to "1".</param>
	/// <param name="g">The <b>Green</b> channel with a range from "0" to "1".</param>
	/// <param name="b">The <b>Blue</b> channel with a range from "0" to "1".</param>
	/// <param name="a">
	/// The <b>Alpha</b> channel with a range from "0" to "1".<br/>
	/// <i>Default value is "1"</i>.
	/// </param>
	public URGB(decimal r, decimal g, decimal b, decimal a = 1)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	protected bool Equals(URGB other) => R == other.R && G == other.G && B == other.B && A == other.A;

	public override bool Equals(object obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((URGB)obj);
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
