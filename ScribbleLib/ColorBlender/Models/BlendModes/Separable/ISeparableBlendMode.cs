namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents a <see href="https://www.w3.org/TR/compositing-1/#blendingseparable">Separable</see> blending.
/// </summary>
public interface ISeparableBlendMode : IBlendMode
{
	/// <summary>
	/// Blends two color's channels into one.
	/// </summary>
	/// <param name="backdrop">The background color channel.</param>
	/// <param name="source">The foreground color channel.</param>
	/// <returns>The blended color channel.</returns>
	decimal Blend(decimal backdrop, decimal source);
}
