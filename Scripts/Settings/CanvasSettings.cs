using Godot;
using ScribbleLib;

namespace Scribble;

public class CanvasSettings
{
    /// <summary>
    /// Is the background a single solid color
    /// </summary>
    public bool BG_IsSolid { get; set; } = false;

    /// <summary>
    /// Background resolution multiplier
    /// </summary>
    public int BG_ResolutionMult { get; set; } = 2;

    float bgBrightnessAdd;
    Color bgBrightnessAddColor;
    /// <summary>
    /// Background resolution multiplier
    /// </summary>
    public float BG_BrightnessAdd
    {
        get => bgBrightnessAdd;
        set
        {
            bgBrightnessAdd = value;
            bgBrightnessAddColor = ColorTools.GrayscaleColor(bgBrightnessAdd);
        }
    }

    Color bgPrimary = ColorTools.GrayscaleColor(0.6f);
    /// <summary>
    /// Background primary color
    /// </summary>
    public Color BG_Primary
    {
        get => bgPrimary + bgBrightnessAddColor;
        set => bgPrimary = value;
    }

    Color bgSecondary = ColorTools.GrayscaleColor(0.4f);
    /// <summary>
    /// Background secondary color
    /// </summary>
    /// public static Color BG_Primary { 
    public Color BG_Secondary
    {
        get => bgSecondary + bgBrightnessAddColor;
        set => bgSecondary = value;
    }
}
