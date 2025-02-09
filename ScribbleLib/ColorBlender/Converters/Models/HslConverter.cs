using System;
using ColorBlender.Models.ColorModels.HSL;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Converters.Models;

/// <summary>
/// The converter between <see cref="HSL"/> and <see cref="URGB"/> types.
/// </summary>
public class HslConverter : IColorConverter
{
	public URGB ToURGB(object colorModel)
	{
		decimal h = (decimal)((HSL)colorModel).H / 360m;
		decimal s = (decimal)((HSL)colorModel).S / 100m;
		decimal l = (decimal)((HSL)colorModel).L / 100m;

		decimal r = 1, g = 1, b = 1, q, p;

		q = (l < 0.5m) ? l * (1 + s) : l + s - l * s;
		p = 2 * l - q;

		if (l == 0)
		{
			r = 0;
			g = 0;
			b = 0;
		}
		else if (s != 0)
		{
			r = GetHue(p, q, h + 1.0m / 3m);
			g = GetHue(p, q, h);
			b = GetHue(p, q, h - 1.0m / 3m);
		}

		return new URGB(r, g, b);
	}

	public object ToColorModel(URGB colorModel, bool roundOutput, int decimals)
	{
		decimal r = colorModel.R;
		decimal g = colorModel.G;
		decimal b = colorModel.B;
		decimal min = Math.Min(r, Math.Min(g, b));
		decimal max = Math.Max(r, Math.Max(g, b));
		decimal delta = max - min;
		decimal l = (min + max) / 2;
		decimal h, s;

		if (delta == 0)
		{
			h = 0;
			s = 0;
		}
		else
		{
			s = (l <= 0.5m) ? (delta / (min + max)) : (delta / (2 - max - min));

			if (r == max)
			{
				h = (g - b) / 6 / delta;
			}
			else if (g == max)
			{
				h = (1.0m / 3) + ((b - r) / 6 / delta);
			}
			else
			{
				h = (2.0m / 3) + ((r - g) / 6 / delta);
			}

			h = (h < 0) ? ++h : h;
			h = (h > 1) ? --h : h;
		}

		HSL result = new((double)h * 360, (double)s * 100, (double)l * 100, (double)colorModel.A);

		if (roundOutput)
		{
			result.H = Math.Round(result.H, decimals);
			result.S = Math.Round(result.S, decimals);
			result.L = Math.Round(result.L, decimals);
		}

		return result;
	}

	private decimal GetHue(decimal p, decimal q, decimal t)
	{
		decimal value = p;

		if (t < 0)
			t++;
		if (t > 1)
			t--;

		if (t < 1.0m / 6)
			value = p + (q - p) * 6 * t;
		else if (t < 1.0m / 2)
			value = q;
		else if (t < 2.0m / 3)
			value = p + (q - p) * (2.0m / 3 - t) * 6;

		return value;
	}
}
