namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public interface ISvgViewElement : ISvgElement, ISvgExternalResourcesRequired, ISvgFitToViewBox, ISvgZoomAndPan 
	{
		ISvgStringList ViewTarget{get;}
	}
}
