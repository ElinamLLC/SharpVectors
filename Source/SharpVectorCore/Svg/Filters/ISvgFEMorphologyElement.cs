namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEMorphologyElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedEnumeration Operator { get; }
        ISvgAnimatedNumber RadiusX { get; }
        ISvgAnimatedNumber RadiusY { get; }
    }
}
