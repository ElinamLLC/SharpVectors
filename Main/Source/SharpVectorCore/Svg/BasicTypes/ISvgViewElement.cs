using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgViewElement:
		ISvgElement,
		ISvgExternalResourcesRequired,
		ISvgFitToViewBox,
		ISvgZoomAndPan 
	{
		ISvgStringList ViewTarget{get;}
	}
}
