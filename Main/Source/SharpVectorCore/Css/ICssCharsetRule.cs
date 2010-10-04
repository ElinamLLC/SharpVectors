using System;
namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSCharsetRule interface represents a @charset rule in a
	/// CSS style sheet. The value of the encoding attribute does not
	/// affect the encoding of text data in the DOM objects; this 
	/// encoding is always UTF-16. After a stylesheet is loaded, the 
	/// value of the encoding attribute is the value found in the 
	/// @charset rule. If there was no @charset in the original 
	/// document, then no CSSCharsetRule is created. The value of the
	/// encoding attribute may also be used as a hint for the 
	/// encoding used on serialization of the style sheet. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>50</completed>	
	public interface ICssCharsetRule : ICssRule
	{
		/// <summary>
		/// The encoding information used in this @charset rule.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified encoding value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this encoding rule is readonly.</exception>
		string Encoding
		{
			get;
			set;
		}
	}
}
