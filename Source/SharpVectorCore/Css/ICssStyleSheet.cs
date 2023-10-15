using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssStyleSheet</c> interface is a concrete interface used to represent a CSS style sheet 
	/// i.e., a style sheet whose content type is <c>text/css</c>. 
	/// </summary>
	public interface ICssStyleSheet : IStyleSheet
	{
		/// <summary>
		/// Used to delete a rule from the style sheet.
		/// </summary>
		/// <param name="index">The index within the style sheet's rule list of the rule to remove.</param>
		/// <exception cref="DomException">
		/// <c>INDEX_SIZE_ERR:</c> Raised if the specified index does not correspond to a rule in the style sheet's rule list.
		/// </exception>
		/// <exception cref="DomException">
		/// <c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this style sheet is readonly.
		/// </exception>
		void DeleteRule(ulong index);

		/// <summary>
		/// Used to insert a new rule into the style sheet. The new rule now becomes part of the cascade.
		/// </summary>
		/// <param name="rule">The parsable text representing the rule. For rule sets this contains both the selector 
		/// and the style declaration. For at-rules, this specifies both the at-identifier and the rule content.</param>
		/// <param name="index">The index within the style sheet's rule list of the rule before which to insert 
		/// the specified rule. If the specified index is equal to the length of the style sheet's rule collection, 
		/// the rule will be added to the end of the style sheet.</param>
		/// <returns>The index within the style sheet's rule collection of the newly inserted rule.</returns>
		/// <exception cref="DomException">
		/// <c>HIERARCHY_REQUEST_ERR:</c> Raised if the rule cannot be inserted at the specified index 
		/// e.g. if an <c>@import</c> rule is inserted after a standard rule set or other at-rule.</exception>
		/// <exception cref="DomException">
		/// <c>INDEX_SIZE_ERR:</c> Raised if the specified index does not correspond to a rule in the style sheet's rule list.
		/// </exception>
		/// <exception cref="DomException">
		/// <c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this style sheet is readonly.</exception>
		/// <exception cref="DomException">
		/// <c>SYNTAX_ERR:</c> Raised if the specified rule has a syntax error and is unparsable.
		/// </exception>
		ulong InsertRule(string rule, ulong index);
	
		/// <summary>
		/// The list of all CSS rules contained within the style sheet. This includes both rule sets and at-rules.
		/// </summary>
		ICssRuleList CssRules
		{
			get;
		}

        /// <summary>
        /// If this style sheet comes from an <c>@import</c> rule, the ownerRule attribute will 
        /// contain the <see cref="ICssImportRule"/>. 
        /// </summary>
        /// <remarks>
        /// In that case, the <see cref="IStyleSheet.OwnerNode"/> attribute in the <see cref="IStyleSheet"/> interface 
        /// will be <see langword="null"/>. If the style sheet comes from an element or a processing instruction, 
        /// the <c>OwnerRule</c> attribute will be <see langword="null"/> and the <see cref="IStyleSheet.OwnerNode"/> 
        /// attribute will contain the node.
        /// </remarks>
        ICssRule OwnerRule
		{
			get;
		}
	}
}
