using System;

namespace SharpVectors.Dom.Css
{
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ICssValue
	{
		/// <summary>
		/// A string representation of the current value.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error (according to the attached property) or is unparsable</exception>
		/// <exception cref="DomException">INVALID_MODIFICATION_ERR: Raised if the specified CSS string value represents a different type of values than the values allowed by the CSS property.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this value is readonly.</exception>
		string CssText
		{
			get;
			set;
		}
	
		/// <summary>
		/// A code defining the type of the value as defined above
		/// </summary>
		SharpVectors.Dom.Css.CssValueType CssValueType
		{
			get;
		}
	}
}
