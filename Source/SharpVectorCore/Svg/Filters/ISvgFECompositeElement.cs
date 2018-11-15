namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFECompositeElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedString In2 { get; }
        ISvgAnimatedEnumeration Operator { get; }
        ISvgAnimatedNumber K1 { get; }
        ISvgAnimatedNumber K2 { get; }
        ISvgAnimatedNumber K3 { get; }
        ISvgAnimatedNumber K4 { get; }
    }
}
