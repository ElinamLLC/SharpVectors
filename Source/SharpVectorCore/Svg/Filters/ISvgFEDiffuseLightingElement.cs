namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEDiffuseLightingElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedNumber SurfaceScale { get; }
        ISvgAnimatedNumber DiffuseConstant { get; }
        ISvgAnimatedNumber KernelUnitLengthX { get; }
        ISvgAnimatedNumber KernelUnitLengthY { get; }
    }
}
