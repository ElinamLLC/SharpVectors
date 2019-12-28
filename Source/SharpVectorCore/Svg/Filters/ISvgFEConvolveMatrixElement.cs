namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEConvolveMatrixElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedEnumeration EdgeMode { get; }
        ISvgAnimatedInteger OrderX { get; }
        ISvgAnimatedInteger OrderY { get; }
        ISvgAnimatedNumberList KernelMatrix { get; }
        ISvgAnimatedNumber Divisor { get; }
        ISvgAnimatedNumber Bias { get; }
        ISvgAnimatedInteger TargetX { get; }
        ISvgAnimatedInteger TargetY { get; }
        ISvgAnimatedNumber KernelUnitLengthX { get; }
        ISvgAnimatedNumber KernelUnitLengthY { get; }
        ISvgAnimatedBoolean PreserveAlpha { get; }
    }
}
