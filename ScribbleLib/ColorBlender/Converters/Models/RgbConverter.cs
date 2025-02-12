using System;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Models;

/// <summary>
/// The converter between <see cref="RGB"/> and <see cref="URGB"/> types.
/// </summary>
public class RgbConverter : IColorConverter
{
	public URGB ToURGB(object colorModel) => new()
	{
		R = (decimal)((RGB)colorModel).R / 255m,
		G = (decimal)((RGB)colorModel).G / 255m,
		B = (decimal)((RGB)colorModel).B / 255m,
		A = (decimal)((RGB)colorModel).A
	};

	public object ToColorModel(URGB colorModel, bool roundOutput, int decimals)
	{
		decimal r = colorModel.R * 255;
		decimal g = colorModel.G * 255;
		decimal b = colorModel.B * 255;
		decimal a = colorModel.A;

		if (roundOutput)
		{
			r = Math.Round(r, decimals);
			g = Math.Round(g, decimals);
			b = Math.Round(b, decimals);
		}

		return new RGB
		{
			R = (double)r,
			G = (double)g,
			B = (double)b,
			A = (double)a
		};
	}
}
