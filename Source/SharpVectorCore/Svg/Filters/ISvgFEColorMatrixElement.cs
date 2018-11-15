namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEColorMatrixElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedEnumeration Type { get; }
        ISvgAnimatedNumberList Values { get; }
    }
}
