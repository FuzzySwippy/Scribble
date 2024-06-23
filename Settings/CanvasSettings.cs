using Godot;
using Scribble.ScribbleLib.Extensions;

namespace Scribble.Settings;

public class CanvasSettings
{
	/// <summary>
	/// Is the background a single solid color
	/// </summary>
	public bool BGIsSolid { get; set; } = false;

	/// <summary>
	/// Background resolution multiplier
	/// </summary>
	public int BGResolutionMult { get; set; } = 2;

	private float bgBrightnessAdd;
	private Color bgBrightnessAddColor;
	/// <summary>
	/// Background resolution multiplier
	/// </summary>
	public float BGBrightnessAdd
	{
		get => bgBrightnessAdd;
		set
		{
			bgBrightnessAdd = value;
			bgBrightnessAddColor = ColorTools.GrayscaleColor(bgBrightnessAdd);
		}
	}

	private Color bgPrimary = ColorTools.GrayscaleColor(0.6f);
	/// <summary>
	/// Background primary color
	/// </summary>
	public Color BGPrimary
	{
		get => bgPrimary + bgBrightnessAddColor;
		set => bgPrimary = value;
	}

	private Color bgSecondary = ColorTools.GrayscaleColor(0.4f);
	/// <summary>
	/// Background secondary color
	/// </summary>
	/// public static Color BG_Primary { 
	public Color BGSecondary
	{
		get => bgSecondary + bgBrightnessAddColor;
		set => bgSecondary = value;
	}

	public bool AutosaveEnabled { get; set; } = true;
	public int AutosaveIntervalMinutes { get; set; } = 5;
}
