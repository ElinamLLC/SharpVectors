namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgComponentTransferFunctionElement : ISvgElement
    {
        ISvgAnimatedEnumeration Type { get; }
        ISvgAnimatedNumberList TableValues { get; }
        ISvgAnimatedNumber Slope { get; }
        ISvgAnimatedNumber Intercept { get; }
        ISvgAnimatedNumber Amplitude { get; }
        ISvgAnimatedNumber Exponent { get; }
        ISvgAnimatedNumber Offset { get; }
    }
}
