using System.ComponentModel;

namespace ColorBlender.Models.ColorModels.HEX.Enums;

/// <summary>
/// Represents Alpha channel position.
/// </summary>
public enum EHEXFormat
{
	[Description("The position of the Alpha channel is at the end")]
	HEXA,
	[Description("The position of the Alpha channel is at the start")]
	AHEX
}
