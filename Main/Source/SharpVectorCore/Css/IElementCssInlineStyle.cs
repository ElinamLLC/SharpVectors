// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>	

using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// Inline style information attached to elements is exposed 
	/// through the style attribute. 
	/// </summary>
    /// <remarks>
    /// This represents the contents of the STYLE attribute for HTML elements 
    /// (or elements in other schemas or DTDs which use the STYLE attribute in the same 
	/// way). The expectation is that an instance of the 
	/// ElementCSSInlineStyle interface can be obtained by using 
	/// binding-specific casting methods on an instance of the 
	/// Element interface when the element supports inline CSS 
	/// style informations. 
    /// </remarks>
	public interface IElementCssInlineStyle
	{
		/// <summary>
		/// The style attribute
		/// </summary>
		ICssStyleDeclaration Style
		{
			get;
		}
	}
}
