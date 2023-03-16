using Godot;
using ScribbleLib;
using System;

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

        static int bgBrightnessAdd = 0;
        static Color bgBrightnessAddColor = new();
        /// <summary>
        /// Background resolution multiplier
        /// </summary>
        public static int BG_BrightnessAdd
        {
            get => bgBrightnessAdd;
            set
            {
                bgBrightnessAdd = value;
                bgBrightnessAddColor = ColorTools.Grayscale(bgBrightnessAdd);
            }
        }

        static Color bgPrimary = ColorTools.Grayscale(0.7f);
        /// <summary>
        /// Background primary color
        /// </summary>
        public static Color BG_Primary { 
			get => bgPrimary + bgBrightnessAddColor; 
			set => bgPrimary = value; 
		}

        static Color bgSecondary = ColorTools.Grayscale(0.7f);
        /// <summary>
        /// Background secondary color
        /// </summary>
        /// public static Color BG_Primary { 
        public static Color BG_Secondary
        {
            get => bgSecondary + bgBrightnessAddColor;
            set => bgSecondary = value;
        }
    }
}
