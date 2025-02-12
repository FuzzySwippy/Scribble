using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingcolorburn">Color Burn</see> blend mode.
/// </summary>
public class ColorBurn : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) =>
		backdrop == 1
			? 1
			: source == 0
				? 0
				: 1 - Math.Min(1, (1 - backdrop) / source);
}
