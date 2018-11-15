namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEOffsetElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedNumber Dx { get; }
        ISvgAnimatedNumber Dy { get; }
    }
}
