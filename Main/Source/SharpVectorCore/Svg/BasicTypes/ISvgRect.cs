// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Rectangles are defined as consisting of a (x,y) coordinate pair identifying a minimum X value, a minimum Y value, and a width and height, which are usually constrained to be non-negative. 
	/// </summary>
	public interface ISvgRect
	{
		double X { get; set; }
		double Y { get; set; }
		double Width { get; set; }
		double Height { get; set; }
	}
}
