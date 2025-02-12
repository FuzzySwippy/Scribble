namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendinghardlight">Hard Light</see> blend mode.
/// </summary>
public class HardLight : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) =>
		source <= 0.5m
			? new Multiply().Blend(backdrop, 2 * source)
			: new Screen().Blend(backdrop, 2 * source - 1);
}
