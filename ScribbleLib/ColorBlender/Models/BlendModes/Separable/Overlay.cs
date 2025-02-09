namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingoverlay">Overlay</see> blend mode.
/// </summary>
public class Overlay : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => new HardLight().Blend(source, backdrop);
}
