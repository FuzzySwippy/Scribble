namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingscreen">Screen</see> blend mode.
/// </summary>
public class Screen : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => backdrop + source - backdrop * source;
}
