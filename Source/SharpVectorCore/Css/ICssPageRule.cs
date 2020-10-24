namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssPageRule</c> interface represents a <c>@page</c> rule within a CSS style sheet. 
	/// The <c>@page</c> rule is used to specify the dimensions, orientation, margins, etc. of a page box for paged media. 
	/// </summary>
	public interface ICssPageRule : ICssRule
	{
		/// <summary>
		/// The declaration-block of this rule.
		/// </summary>
		ICssStyleDeclaration Style
		{
			get;
		}
	
		/// <summary>
		/// The parsable textual representation of the page selector for the rule.
		/// </summary>
		/// <exception cref="DomException">
        /// <c>SYNTAX_ERR</c>: Raised if the specified CSS string value has a syntax error and is unparsable.
        /// </exception>
		/// <exception cref="DomException">
		/// <c>NO_MODIFICATION_ALLOWED_ERR</c>: Raised if this rule is readonly.
		/// </exception>
		string SelectorText
		{
			get;
			set;
		}
	}
}
