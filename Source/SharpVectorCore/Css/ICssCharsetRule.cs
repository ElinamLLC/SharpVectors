namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssCharsetRule</c> interface represents a <c>@charset</c> rule in a CSS style sheet. The value of the 
	/// encoding attribute does not affect the encoding of text data in the DOM objects; this encoding is 
	/// always UTF-16. After a stylesheet is loaded, the value of the encoding attribute is the value found in the 
	/// <c>@charset</c> rule. If there was no <c>@charset</c> in the original document, then no <c>ICssCharsetRule</c> is created. 
	/// The value of the encoding attribute may also be used as a hint for the 
	/// encoding used on serialization of the style sheet. 
	/// </summary>
	public interface ICssCharsetRule : ICssRule
	{
		/// <summary>
		/// The encoding information used in this <c>@charset</c> rule.
		/// </summary>
		/// <exception cref="DomException">
		/// <c>SYNTAX_ERR</c>: Raised if the specified encoding value has a syntax error and is unparsable.
		/// </exception>
		/// <exception cref="DomException">
		/// <c>NO_MODIFICATION_ALLOWED_ERR</c>: Raised if this encoding rule is readonly.
		/// </exception>
		string Encoding
		{
			get;
			set;
		}
	}
}
