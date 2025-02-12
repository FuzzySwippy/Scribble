using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingsoftlight">Soft Light</see> blend mode.
/// </summary>
public class SoftLight : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) =>
		source <= 0.5m
			? backdrop - (1 - 2 * source) * backdrop * (1 - backdrop)
			: backdrop + (2 * source - 1) *
			(backdrop <= 0.25m
				 ? ((16 * backdrop - 12) * backdrop + 4) * backdrop
				 : (decimal)Math.Sqrt((double)backdrop)) -
			backdrop;
}
