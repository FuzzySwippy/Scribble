namespace ColorBlender.Services.AlphaCompose
{
    /// <summary>
    /// Represents an Alpha composition.
    /// </summary>
    public class AlphaComposeService : IAlphaComposeService
    {
        public decimal AlphaCompose(
            decimal backdropAlpha,
            decimal sourceAlpha,
            decimal compositeAlpha,
            decimal backdropColor,
            decimal sourceColor,
            decimal compositeColor)
        {
            return (sourceAlpha * (1 - backdropAlpha) * sourceColor + sourceAlpha * backdropAlpha * compositeColor + (1 - sourceAlpha) * backdropAlpha * backdropColor) / compositeAlpha;
        }
    }
}
