using System.Drawing;
using ColorBlender.Models.ColorModels;
using ColorBlender.Models.ColorModels.HEX;
using ColorBlender.Models.ColorModels.HSL;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Services;

/// <summary>
/// Represents a converter between color types.
/// </summary>
public interface IColorConverterService
{
	/// <summary>
	/// Converts any <see cref="IColorModel"/> into the <see cref="URGB"/> type.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/>.</param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "False"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "3"</i>.
	/// </param>
	/// <returns>The <see cref="URGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	URGB ToUrgb(IColorModel colorModel, bool roundOutput = false, int decimals = 2);

	/// <summary>
	/// Converts a <see cref="Color"/> into the <see cref="URGB"/> type.
	/// </summary>
	/// <param name="colorModel">System <see cref="Color"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "False"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "3"</i>.
	/// </param>
	/// <returns>The <see cref="URGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	URGB ToUrgb(Color colorModel, bool roundOutput = false, int decimals = 2);

	/// <summary>
	/// Converts any <see cref="IColorModel"/> into the <see cref="RGB"/> type.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/>.</param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="RGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	RGB ToRgb(IColorModel colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts a <see cref="Color"/> into the <see cref="RGB"/> type.
	/// </summary>
	/// <param name="colorModel">System <see cref="Color"/>.</param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="RGB"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	RGB ToRgb(Color colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts any <see cref="IColorModel"/> into the <see cref="HSL"/> type.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="HSL"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	HSL ToHsl(IColorModel colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts a <see cref="Color"/> into the <see cref="HSL"/> type.
	/// </summary>
	/// <param name="colorModel">System <see cref="Color"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="HSL"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	HSL ToHsl(Color colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts any <see cref="IColorModel"/> into the <see cref="HEX"/> type.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="HEX"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	HEX ToHex(IColorModel colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts a <see cref="Color"/> into the <see cref="HEX"/> type.
	/// </summary>
	/// <param name="colorModel">System <see cref="Color"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="HEX"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	HEX ToHex(Color colorModel, bool roundOutput = true, int decimals = 0);

	/// <summary>
	/// Converts any <see cref="IColorModel"/> into the <see cref="Color"/> type.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/></param>
	/// <param name="roundOutput">
	/// Output color channels rounding.<br/>
	/// <i>Default value is "True"</i>.
	/// </param>
	/// <param name="decimals">
	/// The number of decimal places in the return value.<br/>
	/// <i>Default value is "0"</i>.
	/// </param>
	/// <returns>The <see cref="Color"/> color.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	Color ToColor(IColorModel colorModel, bool roundOutput = true, int decimals = 0);
}
