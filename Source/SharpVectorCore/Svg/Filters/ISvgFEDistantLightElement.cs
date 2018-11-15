namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEDistantLightElement : ISvgElement
    {
        ISvgAnimatedNumber Azimuth { get; }
        ISvgAnimatedNumber Elevation { get; }
    }
}
