// <developer>niklas@protocol7.com</developer>
// <completed>40</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Interface SvgLocatable is for all elements which either have a 
	/// transform attribute or don't have a transform attribute but whose
	/// content can have a bounding box in current user space. 
	/// </summary>
	public interface ISvgLocatable
	{
		ISvgElement NearestViewportElement{get;}
		ISvgElement FarthestViewportElement{get;}
		ISvgRect GetBBox();
		ISvgMatrix GetCTM();
		ISvgMatrix GetScreenCTM();
		ISvgMatrix GetTransformToElement(ISvgElement element);
	}
}