// <developer>niklas@protocol7.com</developer>
// <completed>25</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Interface SvgUriReference defines an interface which applies to all elements which have the collection of XLink attributes, such as xlink:href, which define a URI reference. 
	/// </summary>
	public interface ISvgUriReference
	{
		ISvgAnimatedString Href
        {
            get;
        }
	}
}
