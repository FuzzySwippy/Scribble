using System;
using ColorBlender.Converters.Models;
using ColorBlender.Models.ColorModels;
using Color = System.Drawing.Color;

namespace ColorBlender.Converters.Factories;

/// <summary>
/// Represents a Factory for creating color converters based on the provided color type.
/// </summary>
public interface IColorConverterFactory
{
	/// <summary>
	/// Returns color converter for provided <see cref="IColorModel"/> type.
	/// </summary>
	/// <typeparam name="T">Any <see cref="IColorModel"/> type.</typeparam>
	/// <returns>Color converter for provided color type.</returns>
	IColorConverter GetColorConverter<T>() where T : IColorModel, new();

	/// <summary>
	/// Returns color converter for provided <see cref="IColorModel"/> type.
	/// </summary>
	/// <param name="type">Any <see cref="IColorModel"/> and <see cref="Color"/> type.</param>
	/// <remarks>Also accepts <see cref="Color"/>.</remarks>
	/// <returns>Color converter for provided color type.</returns>
	IColorConverter GetColorConverter(Type type);
}
