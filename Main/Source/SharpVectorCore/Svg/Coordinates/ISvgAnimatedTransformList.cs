// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for the various attributes which specify a set of transformations, such as the transform attribute which is available for many of Svg's elements, and which can be animated.
	/// </summary>
	public interface ISvgAnimatedTransformList
	{
		 ISvgTransformList BaseVal { get; }
		 ISvgTransformList AnimVal { get; }
	}
}
