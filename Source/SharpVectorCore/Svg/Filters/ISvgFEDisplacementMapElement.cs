namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEDisplacementMapElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedString In2 { get; }
        ISvgAnimatedNumber Scale { get; }
        ISvgAnimatedEnumeration XChannelSelector { get; }
        ISvgAnimatedEnumeration YChannelSelector { get; }
    }
}
