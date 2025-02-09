using System.ComponentModel;

namespace ColorBlender.Models.ColorModels.HEX.Enums;

/// <summary>
/// Represents Alpha channel visibility.
/// </summary>
public enum EHEXOutputFormat
{
	[Description("HEXA color with optional Alpha value. The Alpha value will only be displayed if color does not have the maximum opacity.")]
	HEXAOpt,
	[Description("HEXA color with displayed Alpha value. The Alpha value will always be displayed.")]
	HEXAConst,
	[Description("AHEX color with optional Alpha value. The Alpha value will only be displayed if color does not have the maximum opacity.")]
	OptAHEX,
	[Description("AHEX color with displayed Alpha value. The Alpha value will always be displayed.")]
	ConstAHEX
}