namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEBlendElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedString In2 { get; }
        ISvgAnimatedEnumeration Mode { get; }
    }
}
