using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSStyleRule interface represents a single rule set 
	/// in a CSS style sheet. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>90</completed>	
	public interface ICssStyleRule : ICssRule
	{
		/// <summary>
		/// The textual representation of the selector for the rule set. The implementation may have stripped out insignificant whitespace while parsing the selector.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this rule is readonly.</exception>
		string SelectorText
		{
			get;
			set;
		}

		/// <summary>
		/// The declaration-block of this rule set.
		/// </summary>
		ICssStyleDeclaration Style
		{
			get;
		}
	}
}
