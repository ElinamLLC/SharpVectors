namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFESpotLightElement : ISvgElement
    {
        ISvgAnimatedNumber X { get; }
        ISvgAnimatedNumber Y { get; }
        ISvgAnimatedNumber Z { get; }
        ISvgAnimatedNumber PointsAtX { get; }
        ISvgAnimatedNumber PointsAtY { get; }
        ISvgAnimatedNumber PointsAtZ { get; }
        ISvgAnimatedNumber SpecularExponent { get; }
        ISvgAnimatedNumber LimitingConeAngle { get; }
    }
}
