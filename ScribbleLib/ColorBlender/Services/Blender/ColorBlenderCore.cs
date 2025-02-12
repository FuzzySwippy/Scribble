using ColorBlender.Models.BlendModes;
using ColorBlender.Models.BlendModes.NonSeparable;
using ColorBlender.Models.BlendModes.Separable;
using ColorBlender.Models.ColorModels.RGB;
using ColorBlender.Services.AlphaCompose;

namespace ColorBlender.Services.Blender;

public class ColorBlenderCore : IColorBlenderCore
{
	private readonly AlphaComposeService alphaComposeService = new();

	public URGB PerformBlend<T>(URGB backdrop, URGB source) where T : IBlendMode, new()
	{
		IBlendMode blendModeInstance = new T();

		return typeof(ISeparableBlendMode).IsAssignableFrom(typeof(T))
			? PerformSeparableBlend(backdrop, source, (ISeparableBlendMode)blendModeInstance)
			: PerformNonSeparableBlend(backdrop, source, (INonSeparableBlendMode)blendModeInstance);
	}

	/// <summary>
	/// Concrete blend method for separable blend modes.
	/// </summary>
	/// <param name="backdrop">The backdrop color</param>
	/// <param name="source">The source color</param>
	/// <param name="blendMode">Any <see cref="ISeparableBlendMode"/>.</param>
	/// <returns>The blended color.</returns>
	private URGB PerformSeparableBlend(URGB backdrop, URGB source, ISeparableBlendMode blendMode)
	{
		decimal a = source.A + backdrop.A - source.A * backdrop.A;

		URGB blendedColor = new()
		{
			R = blendMode.Blend(backdrop.R, source.R),
			G = blendMode.Blend(backdrop.G, source.G),
			B = blendMode.Blend(backdrop.B, source.B),
			A = a
		};

		blendedColor.R = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.R, source.R, blendedColor.R);
		blendedColor.G = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.G, source.G, blendedColor.G);
		blendedColor.B = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.B, source.B, blendedColor.B);

		return blendedColor;
	}

	/// <summary>
	/// Concrete blend method for non-separable blend modes.
	/// </summary>
	/// <param name="backdrop">The backdrop color.</param>
	/// <param name="source">The source color.</param>
	/// <param name="blendMode">Any <see cref="INonSeparableBlendMode"/>.</param>
	/// <returns>The blended color.</returns>
	private URGB PerformNonSeparableBlend(URGB backdrop, URGB source, INonSeparableBlendMode blendMode)
	{
		decimal a = source.A + backdrop.A - source.A * backdrop.A;

		URGB blendedColor = blendMode.Blend(backdrop, source);

		blendedColor.R = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.R, source.R, blendedColor.R);
		blendedColor.G = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.G, source.G, blendedColor.G);
		blendedColor.B = alphaComposeService.AlphaCompose(backdrop.A, source.A, a, backdrop.B, source.B, blendedColor.B);
		blendedColor.A = a;

		return blendedColor;
	}
}
