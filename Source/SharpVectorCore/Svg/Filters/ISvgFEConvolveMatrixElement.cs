namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEConvolveMatrixElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedInteger OrderX { get; }
        ISvgAnimatedInteger OrderY { get; }
        ISvgAnimatedNumberList KernelMatrix { get; }
        ISvgAnimatedNumber Divisor { get; }
        ISvgAnimatedNumber Bias { get; }
        ISvgAnimatedInteger TargetX { get; }
        ISvgAnimatedInteger TargetY { get; }
        ISvgAnimatedEnumeration EdgeMode { get; }
        ISvgAnimatedNumber KernelUnitLengthX { get; }
        ISvgAnimatedNumber KernelUnitLengthY { get; }
        bool PreserveAlpha { get; }
    }
}
