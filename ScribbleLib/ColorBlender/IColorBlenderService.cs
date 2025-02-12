using System.Drawing;
using ColorBlender.Converters.Models;
using ColorBlender.Models.ColorModels;

namespace ColorBlender;

public interface IColorBlenderService
{
	#region ColorBurn
	/// <summary>
	/// Blends two colors with the <b>Color Burn</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorBurn(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color Burn</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorBurn(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color Burn</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorBurn(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Color Burn</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorBurn(Color backdrop, Color source);
	#endregion

	#region ColorDodge
	/// <summary>
	/// Blends two colors with the <b>Color Dodge</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorDodge(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color Dodge</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorDodge(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color Dodge</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorDodge(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Color Dodge</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor ColorDodge(Color backdrop, Color source);
	#endregion

	#region Darken
	/// <summary>
	/// Blends two colors with the <b>Darken</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Darken(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Darken</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Darken(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Darken</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Darken(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Darken</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Darken(Color backdrop, Color source);
	#endregion

	#region Difference
	/// <summary>
	/// Blends two colors with the <b>Difference</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Difference(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Difference</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Difference(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Difference</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Difference(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Difference</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Difference(Color backdrop, Color source);
	#endregion

	#region Exclusion
	/// <summary>
	/// Blends two colors with the <b>Exclusion</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Exclusion(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Exclusion</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Exclusion(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Exclusion</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Exclusion(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Exclusion</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Exclusion(Color backdrop, Color source);
	#endregion

	#region HardLight
	/// <summary>
	/// Blends two colors with the <b>HardLight</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor HardLight(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>HardLight</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor HardLight(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>HardLight</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor HardLight(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>HardLight</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor HardLight(Color backdrop, Color source);
	#endregion

	#region Lighten
	/// <summary>
	/// Blends two colors with the <b>Lighten</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Lighten(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Lighten</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Lighten(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Lighten</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Lighten(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Lighten</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Lighten(Color backdrop, Color source);
	#endregion

	#region Multiply
	/// <summary>
	/// Blends two colors with the <b>Multiply</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Multiply(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Multiply</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Multiply(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Multiply</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Multiply(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Multiply</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Multiply(Color backdrop, Color source);
	#endregion

	#region Normal
	/// <summary>
	/// Blends two colors with the <b>Normal</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Normal(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Normal</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Normal(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Normal</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Normal(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Normal</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Normal(Color backdrop, Color source);
	#endregion

	#region Overlay
	/// <summary>
	/// Blends two colors with the <b>Overlay</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Overlay(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Overlay</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Overlay(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Overlay</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Overlay(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Overlay</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Overlay(Color backdrop, Color source);
	#endregion

	#region Screen
	/// <summary>
	/// Blends two colors with the <b>Screen</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Screen(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Screen</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Screen(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Screen</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Screen(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Screen</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Screen(Color backdrop, Color source);
	#endregion

	#region SoftLight
	/// <summary>
	/// Blends two colors with the <b>Soft Light</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor SoftLight(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Soft Light</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor SoftLight(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Soft Light</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor SoftLight(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Soft Light</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor SoftLight(Color backdrop, Color source);
	#endregion



	#region Color
	/// <summary>
	/// Blends two colors with the <b>Color</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Color(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Color(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Color</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Color(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Color</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Color(Color backdrop, Color source);
	#endregion

	#region Hue
	/// <summary>
	/// Blends two colors with the <b>Hue</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Hue(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Hue</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Hue(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Hue</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Hue(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Hue</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Hue(Color backdrop, Color source);
	#endregion

	#region Luminosity
	/// <summary>
	/// Blends two colors with the <b>Luminosity</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Luminosity(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Luminosity</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Luminosity(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Luminosity</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Luminosity(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Luminosity</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Luminosity(Color backdrop, Color source);
	#endregion

	#region Saturation
	/// <summary>
	/// Blends two colors with the <b>Saturation</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Saturation(IColorModel backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Saturation</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Saturation(Color backdrop, IColorModel source);

	/// <summary>
	/// Blends two colors with the <b>Saturation</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Saturation(IColorModel backdrop, Color source);

	/// <summary>
	/// Blends two colors with the <b>Saturation</b> blend mode.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	UniformColor Saturation(Color backdrop, Color source);
	#endregion
}
