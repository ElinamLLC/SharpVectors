// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgRectElement interface corresponds to the 'image' element. 
	/// </summary>
	public interface ISvgImageElement : ISvgElement, ISvgTests, ISvgStylable,
		ISvgTransformable, ISvgLangSpace, ISvgExternalResourcesRequired
	{
		/// <summary>
		/// Corresponds to attribute x on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength X { get; }

		/// <summary>
		/// Corresponds to attribute y on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Y { get; }

		/// <summary>
		/// Corresponds to attribute width on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Width { get; }
		
		/// <summary>
		/// Corresponds to attribute height on the given 'rect' element.
		/// </summary>
		ISvgAnimatedLength Height { get; }

		ISvgAnimatedPreserveAspectRatio PreserveAspectRatio { get; }

        ISvgColorProfileElement ColorProfile
        {
            get;
        }
	}
}
