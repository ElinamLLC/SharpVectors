namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFETurbulenceElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedNumber BaseFrequencyX { get; }
        ISvgAnimatedNumber BaseFrequencyY { get; }
        ISvgAnimatedInteger NumOctaves { get; }
        ISvgAnimatedNumber Seed { get; }
        ISvgAnimatedEnumeration StitchTiles { get; }
        ISvgAnimatedEnumeration Type { get; }
    }
}
