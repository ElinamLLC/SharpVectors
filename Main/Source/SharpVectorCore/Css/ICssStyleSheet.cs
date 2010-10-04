using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSStyleSheet interface is a concrete interface used to 
	/// represent a CSS style sheet i.e., a style sheet whose 
	/// content type is "text/css". 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>70</completed>	
	public interface ICssStyleSheet : Stylesheets.IStyleSheet
	{
		/// <summary>
		/// Used to delete a rule from the style sheet.
		/// </summary>
		/// <param name="index">The index within the style sheet's rule list of the rule to remove.</param>
		/// <exception cref="DomException">INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the style sheet's rule list.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this style sheet is readonly.</exception>
		void DeleteRule(ulong index);
	
		/// <summary>
		/// Used to insert a new rule into the style sheet. The new rule now becomes part of the cascade.
		/// </summary>
		/// <param name="rule">The parsable text representing the rule. For rule sets this contains both the selector and the style declaration. For at-rules, this specifies both the at-identifier and the rule content.</param>
		/// <param name="index">The index within the style sheet's rule list of the rule before which to insert the specified rule. If the specified index is equal to the length of the style sheet's rule collection, the rule will be added to the end of the style sheet.</param>
		/// <returns>The index within the style sheet's rule collection of the newly inserted rule.</returns>
		/// <exception cref="DomException">HIERARCHY_REQUEST_ERR: Raised if the rule cannot be inserted at the specified index e.g. if an @import rule is inserted after a standard rule set or other at-rule.</exception>
		/// <exception cref="DomException">INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the style sheet's rule list.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this style sheet is readonly.</exception>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified rule has a syntax error and is unparsable.</exception>
		ulong InsertRule(string rule, ulong index);
	
		/// <summary>
		/// The list of all CSS rules contained within the style sheet. This includes both rule sets and at-rules.
		/// </summary>
		SharpVectors.Dom.Css.ICssRuleList CssRules
		{
			get;
			set;
		}
	
		/// <summary>
		/// If this style sheet comes from an @import rule, the ownerRule attribute will contain the CSSImportRule. In that case, the ownerNode attribute in the StyleSheet interface will be null. If the style sheet comes from an element or a processing instruction, the ownerRule attribute will be null and the ownerNode attribute will contain the Node.
		/// </summary>
		SharpVectors.Dom.Css.ICssRule OwnerRule
		{
			get;
		}
	}
}
