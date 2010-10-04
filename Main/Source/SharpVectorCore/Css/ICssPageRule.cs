using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSPageRule interface represents a @page rule within
	/// a CSS style sheet. The @page rule is used to specify the
	/// dimensions, orientation, margins, etc. of a page box for
	/// paged media. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>80</completed>	
	public interface ICssPageRule : ICssRule
	{
		/// <summary>
		/// The declaration-block of this rule.
		/// </summary>
		SharpVectors.Dom.Css.ICssStyleDeclaration Style
		{
			get;
		}
	
		/// <summary>
		/// The parsable textual representation of the page selector for the rule.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this rule is readonly.</exception>
		string SelectorText
		{
			get;
			set;
		}
	}
}
