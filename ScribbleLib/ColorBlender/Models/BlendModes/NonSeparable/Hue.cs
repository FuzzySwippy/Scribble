using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendinghue">Hue</see> blend mode.
/// </summary>
public class Hue : INonSeparableBlendMode
{
	private readonly NonSeparableHelpers nonSeparableHelpers = new();

	public URGB Blend(URGB backdrop, URGB source) =>
		nonSeparableHelpers.SetLuminosity(
			nonSeparableHelpers.SetSaturation(source, nonSeparableHelpers.GetSaturation(backdrop)),
			nonSeparableHelpers.GetLuminosity(backdrop));
}
