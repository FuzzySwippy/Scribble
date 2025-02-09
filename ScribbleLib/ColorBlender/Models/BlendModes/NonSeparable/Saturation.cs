using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingsaturation">Saturation</see> blend mode.
/// </summary>
public class Saturation : INonSeparableBlendMode
{
	private readonly NonSeparableHelpers nonSeparableHelpers = new();

	public URGB Blend(URGB backdrop, URGB source) => nonSeparableHelpers.SetLuminosity(
			nonSeparableHelpers.SetSaturation(backdrop, nonSeparableHelpers.GetSaturation(source)),
			nonSeparableHelpers.GetLuminosity(backdrop));
}
