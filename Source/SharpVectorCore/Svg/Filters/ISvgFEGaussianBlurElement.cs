namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEGaussianBlurElement : ISvgElement, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedNumber StdDeviationX { get; }
        ISvgAnimatedNumber StdDeviationY { get; }
        void SetStdDeviation(float stdDeviationX, float stdDeviationY);
    }
}
