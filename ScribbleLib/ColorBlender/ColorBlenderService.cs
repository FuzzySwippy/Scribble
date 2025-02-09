using ColorBlender.Converters.Factories;
using ColorBlender.Converters.Models;
using ColorBlender.Models.BlendModes.NonSeparable;
using ColorBlender.Models.BlendModes.Separable;
using ColorBlender.Models.ColorModels;
using ColorBlender.Services.Blender;
using ColorBlender.Models.ColorModels.RGB;
using Color = System.Drawing.Color;

namespace ColorBlender;

public class ColorBlenderService : IColorBlenderService
{
	private readonly ColorBlenderCore colorBlenderCore = new();
	private readonly ColorConverterFactory colorConverterFactory = new();

	#region ColorBurn
	public UniformColor ColorBurn(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorBurn>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorBurn(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorBurn>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorBurn(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorBurn>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorBurn(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorBurn>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region ColorDodge
	public UniformColor ColorDodge(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorDodge>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorDodge(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorDodge>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorDodge(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorDodge>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor ColorDodge(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<ColorDodge>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Darken
	public UniformColor Darken(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Darken>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Darken(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Darken>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Darken(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Darken>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Darken(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Darken>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Difference
	public UniformColor Difference(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Difference>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Difference(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Difference>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Difference(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Difference>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Difference(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Difference>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Exclusion
	public UniformColor Exclusion(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Exclusion>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Exclusion(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Exclusion>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Exclusion(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Exclusion>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Exclusion(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Exclusion>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region HardLight
	public UniformColor HardLight(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<HardLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor HardLight(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<HardLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor HardLight(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<HardLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor HardLight(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<HardLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Lighten
	public UniformColor Lighten(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Lighten>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Lighten(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Lighten>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Lighten(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Lighten>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Lighten(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Lighten>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Multiply
	public UniformColor Multiply(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Multiply>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Multiply(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Multiply>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Multiply(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Multiply>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Multiply(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Multiply>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Normal
	public UniformColor Normal(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Normal>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Normal(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Normal>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Normal(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Normal>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Normal(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Normal>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Overlay
	public UniformColor Overlay(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Overlay>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Overlay(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Overlay>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Overlay(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Overlay>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Overlay(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Overlay>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Screen
	public UniformColor Screen(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Screen>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Screen(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Screen>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Screen(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Screen>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Screen(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Screen>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region SoftLight
	public UniformColor SoftLight(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<SoftLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor SoftLight(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<SoftLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor SoftLight(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<SoftLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor SoftLight(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<SoftLight>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion



	#region Color
	public UniformColor Color(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Models.BlendModes.NonSeparable.Color>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Color(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Models.BlendModes.NonSeparable.Color>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Color(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Models.BlendModes.NonSeparable.Color>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Color(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Models.BlendModes.NonSeparable.Color>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Hue
	public UniformColor Hue(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Hue>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Hue(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Hue>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Hue(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Hue>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Hue(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Hue>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Luminosity
	public UniformColor Luminosity(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Luminosity>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Luminosity(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Luminosity>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Luminosity(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Luminosity>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Luminosity(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Luminosity>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion

	#region Saturation
	public UniformColor Saturation(IColorModel backdrop, IColorModel source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Saturation>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Saturation(Color backdrop, IColorModel source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = colorConverterFactory.GetColorConverter(source.GetType()).ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Saturation>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Saturation(IColorModel backdrop, Color source)
	{
		URGB urgbBackdrop = colorConverterFactory.GetColorConverter(backdrop.GetType()).ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Saturation>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}

	public UniformColor Saturation(Color backdrop, Color source)
	{
		URGB urgbBackdrop = new ColorConverter().ToURGB(backdrop);
		URGB urgbSource = new ColorConverter().ToURGB(source);

		URGB result = colorBlenderCore.PerformBlend<Saturation>(urgbBackdrop, urgbSource);
		return new UniformColor(result);
	}
	#endregion
}
