using System;
using System.Collections.Generic;
using System.Linq;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable;

/// <summary>
/// Represents several auxiliary functions for <b>Non-Separable</b> blending calculation.
/// </summary>
public class NonSeparableHelpers
{
	/// <summary>
	/// Calculates luminosity of the color.
	/// </summary>
	/// <param name="urgb">The <see cref="URGB"/> color.</param>
	/// <returns>The luminosity of the color.</returns>
	public decimal GetLuminosity(URGB urgb) => 0.3m * urgb.R + 0.59m * urgb.G + 0.11m * urgb.B;

	/// <summary>
	/// Sets luminosity to the color.
	/// </summary>
	/// <param name="urgb">The <see cref="URGB"/> color.</param>
	/// <param name="luminosity">The luminosity value to calculate and set.</param>
	/// <returns>The color with the new luminosity.</returns>
	public URGB SetLuminosity(URGB urgb, decimal luminosity)
	{
		decimal delta = luminosity - GetLuminosity(urgb);

		urgb.R += delta;
		urgb.G += delta;
		urgb.B += delta;

		return ClipColor(urgb);
	}

	/// <summary>
	/// Clips color's channels.
	/// </summary>
	/// <param name="urgb">The <see cref="URGB"/> color to clip channels for.</param>
	/// <returns>The <see cref="URGB"/> with clipped channels.</returns>
	public URGB ClipColor(URGB urgb)
	{
		decimal luminosity = GetLuminosity(urgb);

		decimal lowestChannel = Math.Min(urgb.R, Math.Min(urgb.G, urgb.B));
		decimal highestChannel = Math.Max(urgb.R, Math.Max(urgb.G, urgb.B));

		decimal ClipLowest(decimal channel) => luminosity + (channel - luminosity) * luminosity / (luminosity - lowestChannel);
		decimal ClipHighest(decimal channel) => luminosity + (channel - luminosity) * (1 - luminosity) / (highestChannel - luminosity);

		if (lowestChannel < 0.0m)
		{
			urgb.R = ClipLowest(urgb.R);
			urgb.G = ClipLowest(urgb.G);
			urgb.B = ClipLowest(urgb.B);
		}

		if (highestChannel > 1.0m)
		{
			urgb.R = ClipHighest(urgb.R);
			urgb.G = ClipHighest(urgb.G);
			urgb.B = ClipHighest(urgb.B);
		}

		return urgb;
	}

	/// <summary>
	/// Returns saturation of the color.
	/// </summary>
	/// <param name="urgb">The <see cref="URGB"/> to get saturation from.</param>
	/// <returns>The saturation of the color.</returns>
	public decimal GetSaturation(URGB urgb) => Math.Max(urgb.R, Math.Max(urgb.G, urgb.B)) - Math.Min(urgb.R, Math.Min(urgb.G, urgb.B));

	/// <summary>
	/// Sets saturation to the color.
	/// </summary>
	/// <param name="urgb">The <see cref="URGB"/> color.</param>
	/// <param name="saturation">The saturation value to calculate and set.</param>
	/// <returns>The color with the new saturation.</returns>
	public URGB SetSaturation(URGB urgb, decimal saturation)
	{
		Dictionary<string, decimal> sortedChannels = new()
		{
				{ "r", urgb.R },
				{ "g", urgb.G },
				{ "b", urgb.B },
			};

		sortedChannels = sortedChannels.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

		string channelMin = sortedChannels.Keys.ElementAt(0);
		string channelMid = sortedChannels.Keys.ElementAt(1);
		string channelMax = sortedChannels.Keys.ElementAt(2);

		Dictionary<string, decimal> result = new()
		{
				{ "r", urgb.R },
				{ "g", urgb.G },
				{ "b", urgb.B },
			};

		if (result[channelMax] > result[channelMin])
		{
			result[channelMid] = (result[channelMid] - result[channelMin]) * saturation / (result[channelMax] - result[channelMin]);
			result[channelMax] = saturation;
		}
		else
		{
			result[channelMid] = result[channelMax] = 0;
		}

		result[channelMin] = 0;

		return new URGB(result["r"], result["g"], result["b"], urgb.A);
	}
}
