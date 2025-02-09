namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingnormal">Normal</see> blend mode.
/// </summary>
public class Normal : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => source;
}
