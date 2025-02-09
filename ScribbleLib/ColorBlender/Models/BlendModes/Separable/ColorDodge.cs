using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingcolordodge">Color Dodge</see> blend mode.
/// </summary>
public class ColorDodge : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) =>
		backdrop == 0
			? 0
			: source == 1
				? 1
				: Math.Min(1, backdrop / (1 - source));
}
