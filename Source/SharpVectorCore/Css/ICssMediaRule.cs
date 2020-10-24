using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	///	The <c>ICssMediaRule</c> interface represents a <c>@media</c> rule in a CSS style sheet. 
	///	A <c>@media</c> rule can be used to delimit style rules for specific media types
	/// </summary>
	public interface ICssMediaRule : ICssRule
	{
		/// <summary>
		/// Used to delete a rule from the media block.
		/// </summary>
		/// <param name="index">The index within the media block's rule collection of the rule to remove.</param>
		/// <exception cref="DomException">
        /// <c>INDEX_SIZE_ERR</c>: Raised if the specified index does not correspond to a rule in the media rule list.
        /// </exception>
		/// <exception cref="DomException">
        /// <c>NO_MODIFICATION_ALLOWED_ERR</c>: Raised if this media rule is readonly
        /// </exception>
		void DeleteRule(ulong index);
	
		/// <summary>
		/// Used to insert a new rule into the media block.
		/// </summary>
		/// <param name="rule">The parsable text representing the rule. For rule sets this contains both 
        /// the selector and the style declaration. For at-rules, this specifies both the at-identifier 
        /// and the rule content.
        /// </param>
		/// <param name="index">The index within the media block's rule collection of the rule before which 
        /// to insert the specified rule. If the specified index is equal to the length of the media blocks's 
        /// rule collection, the rule will be added to the end of the media block.
        /// </param>
		/// <returns>The index within the media block's rule collection of the newly inserted rule</returns>
		ulong InsertRule(string rule, ulong index);
	
		/// <summary>
		/// A list of all CSS rules contained within the media block.
		/// </summary>
		ICssRuleList CssRules{get;}
	
		/// <summary>
		/// A list of media types for this rule
		/// </summary>
		IMediaList Media{get;}
	}
}
