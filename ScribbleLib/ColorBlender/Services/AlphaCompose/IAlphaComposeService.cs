namespace ColorBlender.Services.AlphaCompose
{
    public interface IAlphaComposeService
    {
        /// <summary>
        /// Applies the appropriate alpha blending to a blend process.<br/>
        /// <see href="https://www.w3.org/TR/compositing-1/#blending"/>
        /// </summary>
        /// <param name="backdropAlpha">The Alpha channel of the backdrop color.</param>
        /// <param name="sourceAlpha">The Alpha channel of the source color.</param>
        /// <param name="compositeAlpha">The Alpha channel of the composite color.</param>
        /// <param name="backdropColor">A color channel (R, G or B) of the backdrop color.</param>
        /// <param name="sourceColor">A color channel (R, G or B) of the source color.</param>
        /// <param name="compositeColor">A color channel (R, G or B) of the composite color.</param>
        /// <returns>The resulting color channel</returns>
        decimal AlphaCompose(decimal backdropAlpha, decimal sourceAlpha, decimal compositeAlpha, decimal backdropColor, decimal sourceColor, decimal compositeColor);
    }
}