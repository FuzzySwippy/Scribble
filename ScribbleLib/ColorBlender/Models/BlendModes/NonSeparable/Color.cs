using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingcolor">Color</see> blend mode.
/// </summary>
public class Color : INonSeparableBlendMode
{
	private readonly NonSeparableHelpers nonSeparableHelpers = new();

	public URGB Blend(URGB backdrop, URGB source) => nonSeparableHelpers.SetLuminosity(source, nonSeparableHelpers.GetLuminosity(backdrop));
}
