using ColorBlender.Models.BlendModes;
using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Services.Blender
{
    /// <summary>
    /// Represents a blending process.
    /// </summary>
    public interface IColorBlenderCore
    {
        /// <summary>
        /// Blends two colors into one.
        /// </summary>
        /// <typeparam name="T">Any <see cref="IBlendMode"/>.</typeparam>
        /// <param name="backdrop">The backdrop color.</param>
        /// <param name="source">The source color.</param>
        /// <returns>The blended color.</returns>
        URGB PerformBlend<T>(URGB backdrop, URGB source) where T : IBlendMode, new();
    }
}
