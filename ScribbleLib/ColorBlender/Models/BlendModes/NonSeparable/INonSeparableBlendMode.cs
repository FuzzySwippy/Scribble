using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable;

/// <summary>
/// Represents a <see href="https://www.w3.org/TR/compositing-1/#blendingnonseparable">Non-Separable</see> blending.
/// </summary>
public interface INonSeparableBlendMode : IBlendMode
{
	/// <summary>
	/// Blends two colors into one.
	/// </summary>
	/// <param name="backdrop">The background color.</param>
	/// <param name="source">The foreground color.</param>
	/// <returns>The blended color.</returns>
	URGB Blend(URGB backdrop, URGB source);
}
