using System;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Models;

/// <summary>
/// The converter between <see cref="URGB"/> and <see cref="URGB"/> types.<br/>
/// It is used mainly for rounding color channels.
/// </summary>
public class UnitRgbConverter : IColorConverter
{
	public URGB ToURGB(object colorModel) => (URGB)colorModel;

	public object ToColorModel(URGB colorModel, bool roundOutput, int decimals)
	{
		if (roundOutput)
		{
			colorModel.R = Math.Round(colorModel.R, decimals);
			colorModel.G = Math.Round(colorModel.G, decimals);
			colorModel.B = Math.Round(colorModel.B, decimals);
			colorModel.A = Math.Round(colorModel.A, decimals);
		}

		return colorModel;
	}
}
