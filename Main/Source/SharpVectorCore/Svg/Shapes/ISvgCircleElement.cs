using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgCircleElement interface corresponds to the 'circle' element. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>90</completed>
	public interface ISvgCircleElement	: 
		ISvgElement,
		ISvgTests,
		ISvgLangSpace,
		ISvgExternalResourcesRequired,
		ISvgStylable,
		ISvgTransformable,
		IEventTarget
	{
		/// <summary>
		/// Corresponds to attribute cx on the given 'circle' element.
		/// </summary>
		ISvgAnimatedLength Cx{get;}

		/// <summary>
		/// Corresponds to attribute cy on the given 'circle' element.
		/// </summary>
		ISvgAnimatedLength Cy{get;}

		/// <summary>
		/// Corresponds to attribute r on the given 'circle' element.
		/// </summary>
		ISvgAnimatedLength R{get;}

	}
}
