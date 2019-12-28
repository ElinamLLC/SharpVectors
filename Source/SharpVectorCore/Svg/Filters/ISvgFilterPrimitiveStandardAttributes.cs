namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFilterPrimitiveStandardAttributes : ISvgStylable
    {
        ISvgAnimatedLength X { get; }
        ISvgAnimatedLength Y { get; }
        ISvgAnimatedLength Width { get; }
        ISvgAnimatedLength Height { get; }
        ISvgAnimatedString Result { get; }
    }
}
