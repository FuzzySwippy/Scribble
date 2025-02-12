using System;

namespace ColorBlender.Models.BlendModes.Separable;

/// <summary>
/// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendinglighten">Lighten</see> blend mode.
/// </summary>
public class Lighten : ISeparableBlendMode
{
	public decimal Blend(decimal backdrop, decimal source) => Math.Min(Math.Max(backdrop, source), 1);
}
