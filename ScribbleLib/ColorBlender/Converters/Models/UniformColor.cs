using System.Drawing;
using ColorBlender.Models.ColorModels.HEX;
using ColorBlender.Models.ColorModels.HSL;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Models;

/// <summary>
/// Represents a Uniform Color for further conversion to the desired color type.
/// </summary>
public class UniformColor(URGB urgb)
{
	private readonly URGB urgb = urgb;

	/// <summary>
	/// Converts the Uniform Color into the <see cref="URGB"/> color.
	/// </summary>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "False"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "2"</i>.
	/// </param>
	/// <returns>The <see cref="URGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	public URGB ToUrgb(bool roundOutput = false, int decimals = 2) => (URGB)new UnitRgbConverter().ToColorModel(urgb, roundOutput, decimals);

	/// <summary>
	/// Converts the Uniform Color into the <see cref="RGB"/> color.
	/// </summary>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>
	/// </param>
	/// <returns>The <see cref="RGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	public RGB ToRgb(bool roundOutput = true, int decimals = 0) => (RGB)new RgbConverter().ToColorModel(urgb, roundOutput, decimals);

	/// <summary>
	/// Converts the Uniform Color into the <see cref="HSL"/> color.
	/// </summary>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>
	/// </param>
	/// <returns>The <see cref="HSL"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	public HSL ToHsl(bool roundOutput = true, int decimals = 0) => (HSL)new HslConverter().ToColorModel(urgb, roundOutput, decimals);

	/// <summary>
	/// Converts the Uniform Color into the <see cref="HEX"/> color.
	/// </summary>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>
	/// </param>
	/// <returns>The <see cref="HEX"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	public HEX ToHex(bool roundOutput = true, int decimals = 0) => (HEX)new HexConverter().ToColorModel(urgb, roundOutput, decimals);

	/// <summary>
	/// Converts the Uniform Color into the <see cref="Color"/> color.
	/// </summary>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>
	/// </param>
	/// <returns>The <see cref="Color"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	public Color ToColor(bool roundOutput = true, int decimals = 0) => (Color)new ColorConverter().ToColorModel(urgb, roundOutput, decimals);
}