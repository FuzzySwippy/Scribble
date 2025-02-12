using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingdifference">Difference</see> blend mode.
/// </summary>
public class Difference : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => Math.Abs(backdrop - source);
}
