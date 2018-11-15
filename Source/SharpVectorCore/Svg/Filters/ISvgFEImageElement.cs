namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgFEImageElement : ISvgElement, ISvgUriReference, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedPreserveAspectRatio PreserveAspectRatio { get; }
    }
}
