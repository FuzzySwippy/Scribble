using System.Drawing;
using ColorBlender.Models.ColorModels;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Models;

public interface IColorConverter
{
	/// <summary>
	/// Converts any <see cref="IColorModel"/> and <see cref="Color"/> into <see cref="URGB"/>.
	/// </summary>
	/// <param name="colorModel">Any <see cref="IColorModel"/> or <see cref="Color"/>.</param>
	/// <returns><see cref="URGB"/> type of a color.</returns>
	URGB ToURGB(object colorModel);

	/// <summary>
	/// Converts <see cref="URGB"/> color into specific one (<see cref="IColorModel"/>, <see cref="Color"/>) implemented by the converter.
	/// </summary>
	/// <param name="colorModel"><see cref="URGB"/> color type.</param>
	/// <param name="roundOutput">Output color channels rounding.</param>
	/// <param name="decimals">The number of decimal places in the return value.</param>
	/// <returns>Specific converted <see cref="IColorModel"/> or <see cref="Color"/>.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals">decimals</paramref> is less than 0 or greater than 28.</exception>
	object ToColorModel(URGB colorModel, bool roundOutput, int decimals);
}
