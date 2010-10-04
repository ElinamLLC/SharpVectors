using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	///	The CSSMediaRule interface represents a @media rule in a CSS style sheet. A @media rule can be used to delimit style rules for specific media types
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>	 
	public interface ICssMediaRule : ICssRule
	{
		/// <summary>
		/// Used to delete a rule from the media block.
		/// </summary>
		/// <param name="index">The index within the media block's rule collection of the rule to remove.</param>
		/// <exception cref="DomException">INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the media rule list.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this media rule is readonly</exception>
		void DeleteRule(ulong index);
	
		/// <summary>
		/// Used to insert a new rule into the media block.
		/// </summary>
		/// <param name="rule">The parsable text representing the rule. For rule sets this contains both the selector and the style declaration. For at-rules, this specifies both the at-identifier and the rule content.</param>
		/// <param name="index">The index within the media block's rule collection of the rule before which to insert the specified rule. If the specified index is equal to the length of the media blocks's rule collection, the rule will be added to the end of the media block.</param>
		/// <returns>The index within the media block's rule collection of the newly inserted rule</returns>
		ulong InsertRule(string rule, ulong index);
	
		/// <summary>
		/// A list of all CSS rules contained within the media block.
		/// </summary>
		SharpVectors.Dom.Css.ICssRuleList CssRules{get;}
	
		/// <summary>
		/// A list of media types for this rule
		/// </summary>
		SharpVectors.Dom.Stylesheets.IMediaList Media{get;}
	}
}
