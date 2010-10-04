using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgRectElement interface corresponds to the 'rect' element. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ISvgRectElement : 
		ISvgElement,
		ISvgTests,
		ISvgLangSpace,
		ISvgExternalResourcesRequired,
		ISvgStylable,
		ISvgTransformable,
		IEventTarget
			
	{
		/// <summary>
		/// Corresponds to attribute x on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength X{get;}

		/// <summary>
		/// Corresponds to attribute y on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Y{get;}

		/// <summary>
		/// Corresponds to attribute rx on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Rx{get;}
		
		/// <summary>
		/// Corresponds to attribute ry on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Ry{get;}
		
		/// <summary>
		/// Corresponds to attribute width on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Width{get;}
		
		/// <summary>
		/// Corresponds to attribute height on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Height{get;}
	}
}
