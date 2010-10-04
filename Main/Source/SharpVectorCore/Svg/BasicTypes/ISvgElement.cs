// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using SharpVectors.Dom;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// All of the Svg DOM interfaces that correspond directly to 
	/// elements in the Svg language (e.g., the SvgPathElement 
	/// interface corresponds directly to the 'path' element in the 
	/// language) are derivative from base class SvgElement. 
	/// </summary>
	public interface ISvgElement : IElement
	{
		/// <summary>
		/// The value of the id attribute on the given element.
		/// Inheriting class should throw an exception when trying
		/// to update a read only element
		/// </summary>
		string Id 
        { 
            get; set;
        }

		/// <summary>
		/// The nearest ancestor 'svg' element. Null if the given 
		/// element is the outermost 'svg' element.
		/// </summary>
		ISvgSvgElement OwnerSvgElement 
        { 
            get;
        }

		/// <summary>
		///     The element which established the current viewport. 
		///     Often, the nearest ancestor 'svg' element. Null if 
		///     the given element is the outermost 'svg' element.
		/// </summary>
		ISvgElement ViewportElement 
        { 
            get;
        }

        // Support for rendering...

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        bool IsRenderable
        {
            get;
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// </value>
        SvgRenderingHint RenderingHint
        {
            get;
        }
	}
}

