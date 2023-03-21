using Godot;
using ScribbleLib;

namespace Scribble;

public static class Settings
{
    public static class Canvas
    {
        /// <summary>
        /// Is the background a single solid color
        /// </summary>
        public static bool BG_IsSolid { get; set; } = false;

        /// <summary>
        /// Background resolution multiplier
        /// </summary>
        public static int BG_ResolutionMult { get; set; } = 2;

        static float bgBrightnessAdd;
        public static Color bgBrightnessAddColor;
        /// <summary>
        /// Background resolution multiplier
        /// </summary>
        public static float BG_BrightnessAdd
        {
            get => bgBrightnessAdd;
            set
            {
                bgBrightnessAdd = value;
                bgBrightnessAddColor = ColorTools.Grayscale(bgBrightnessAdd);
            }
        }

        public static Color bgPrimary = ColorTools.Grayscale(0.6f);
        /// <summary>
        /// Background primary color
        /// </summary>
        public static Color BG_Primary
        {
            get => bgPrimary + bgBrightnessAddColor;
            set => bgPrimary = value;
        }

        static Color bgSecondary = ColorTools.Grayscale(0.4f);
        /// <summary>
        /// Background secondary color
        /// </summary>
        /// public static Color BG_Primary { 
        public static Color BG_Secondary
        {
            get => bgSecondary + bgBrightnessAddColor;
            set => bgSecondary = value;
        }

        static Canvas()
        {
            //Default value initialization
            BG_BrightnessAdd = 0.2f;
        }
    }

    public static class UI
    {
        public static float ContentScale { get; set; } = 1;
    }
}
