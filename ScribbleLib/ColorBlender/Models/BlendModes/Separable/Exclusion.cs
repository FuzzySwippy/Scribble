namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingexclusion">Exclusion</see> blend mode.
/// </summary>
public class Exclusion : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => backdrop + source - 2 * backdrop * source;
}
