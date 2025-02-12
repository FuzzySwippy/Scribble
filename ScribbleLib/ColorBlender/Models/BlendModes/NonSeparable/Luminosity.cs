using ColorBlender.Models.ColorModels.RGB;

namespace ColorBlender.Models.BlendModes.NonSeparable
{
    /// <summary>
    /// Represents the <see href="https://www.w3.org/TR/compositing-1/#blendingluminosity">Luminosity</see> blend mode.
    /// </summary>
    public class Luminosity : INonSeparableBlendMode
    {
        private readonly NonSeparableHelpers _nonSeparableHelpers = new NonSeparableHelpers();

        public URGB Blend(URGB backdrop, URGB source) => _nonSeparableHelpers.SetLuminosity(backdrop, _nonSeparableHelpers.GetLuminosity(source));
    }
}
