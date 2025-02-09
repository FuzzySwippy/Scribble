using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingdarken">Darken</see> blend mode.
/// </summary>
public class Darken : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => Math.Min(backdrop, source);
}
