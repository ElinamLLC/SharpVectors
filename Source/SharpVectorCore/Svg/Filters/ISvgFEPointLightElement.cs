namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEPointLightElement : ISvgElement
    {
        ISvgAnimatedNumber X { get; }
        ISvgAnimatedNumber Y { get; }
        ISvgAnimatedNumber Z { get; }
    }
}
