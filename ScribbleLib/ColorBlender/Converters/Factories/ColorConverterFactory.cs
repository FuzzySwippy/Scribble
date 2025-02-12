using System;
using System.Collections.Generic;
using System.Drawing;
using ColorBlender.Converters.Models;
using ColorBlender.Models.ColorModels;
using ColorBlender.Models.ColorModels.HEX;
using ColorBlender.Models.ColorModels.HSL;
using ColorBlender.Models.ColorModels.RGB;

using ColorConverter = ColorBlender.Converters.Models.ColorConverter;

namespace ColorBlender.Converters.Factories;

/// <summary>
/// Represents a Factory for creating color converters based on the provided color type.
/// </summary>
public class ColorConverterFactory : IColorConverterFactory
{
	private readonly Dictionary<Type, Type> converters = new()
	{
		{ typeof(Color), typeof(ColorConverter)   },
		{ typeof(HEX),   typeof(HexConverter)     },
		{ typeof(HSL),   typeof(HslConverter)     },
		{ typeof(RGB),   typeof(RgbConverter)     },
		{ typeof(URGB),  typeof(UnitRgbConverter) }
	};

	public IColorConverter GetColorConverter<T>() where T : IColorModel, new() => (IColorConverter)Activator.CreateInstance(converters[typeof(T)]);

	public IColorConverter GetColorConverter(Type type) => (IColorConverter)Activator.CreateInstance(converters[type]);
}
