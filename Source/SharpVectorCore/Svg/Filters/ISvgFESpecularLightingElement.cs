namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFESpecularLightingElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedNumber SurfaceScale { get; }
        ISvgAnimatedNumber SpecularConstant { get; }
        ISvgAnimatedNumber SpecularExponent { get; }

        ISvgAnimatedNumber KernelUnitLengthX { get; }
        ISvgAnimatedNumber KernelUnitLengthY { get; }
    }
}
